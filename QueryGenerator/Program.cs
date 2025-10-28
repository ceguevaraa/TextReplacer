using System.Globalization;
using System.Text;
using System.Text.Json;

namespace QueryGenerator;

internal static class Program
{
    private static readonly string[] ProvinceCodeKeys = ["CODIPROV", "COD_PROV", "CODPROV"];
    private static readonly string[] ProvinceNameKeys = ["NOMPROV", "NOM_PROV", "NAME"];
    private static readonly string[] ProvinceCapitalKeys = ["CAPPROV", "CAPITAL"];
    private static readonly string[] ProvinceAreaKeys = ["AREAP5000", "AREA_P5000"];

    private static readonly string[] MunicipalityCodeKeys =
    [
        "CODIMUNI",
        "CODIMUN",
        "CODMUNI",
        "COD_MUNI",
        "CODIGO",
        "CODI_MUNI"
    ];

    private static readonly string[] MunicipalityNameKeys =
        ["NOMMUNI", "NOM_MUNI", "MUNI_NAME", "NAME"];

    private static readonly string[] MunicipalityCapitalKeys =
        ["CAPMUNI", "CAP_MUNI", "MUNI_CAPITAL"];

    private static readonly string[] MunicipalityAreaKeys =
        ["AREAM5000", "AREA_M5000", "AREA"];

    private static readonly string[] MunicipalityComarcaCodeKeys =
        ["CODCOMAR", "COD_COMAR", "CODI_COMARCA"];

    private static readonly string[] MunicipalityComarcaNameKeys =
        ["NOMCOMAR", "NOM_COMAR", "NOM_COMARCA"];

    private static readonly string[] MunicipalityComarcaCapitalKeys =
        ["CAPCOMAR", "CAP_COMAR", "CAPITAL_COMARCA"];

    private static readonly string[] MunicipalityVegueriaCodeKeys =
        ["CODVEGUE", "COD_VEGUE", "CODI_VEGUERIA"];

    private static readonly string[] MunicipalityVegueriaNameKeys =
        ["NOMVEGUE", "NOM_VEGUE", "NOM_VEGUERIA"];

    private static readonly string[] MunicipalityVegueriaCapitalKeys =
        ["CAPVEGUE", "CAP_VEGUE", "CAPITAL_VEGUERIA"];

    private static readonly string[] ProvinceReferenceKeys =
        ["CODIPROV", "COD_PROV", "PROV_CODE", "CODPROV"];

    private static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: dotnet run --project QueryGenerator <path-to-json>");
            return;
        }

        var path = args[0];

        if (!File.Exists(path))
        {
            Console.Error.WriteLine($"The file '{path}' does not exist.");
            Environment.ExitCode = 1;
            return;
        }

        try
        {
            var json = File.ReadAllText(path);
            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            foreach (var sql in GenerateStatements(root))
            {
                Console.WriteLine(sql);
                Console.WriteLine();
            }
        }
        catch (JsonException jsonEx)
        {
            Console.Error.WriteLine($"Invalid JSON input: {jsonEx.Message}");
            Environment.ExitCode = 1;
        }
    }

    private static IEnumerable<string> GenerateStatements(JsonElement root)
    {
        var features = ResolveFeatures(root);

        foreach (var feature in features)
        {
            if (!feature.TryGetProperty("properties", out var properties))
            {
                continue;
            }

            if (IsProvince(properties))
            {
                var sql = BuildProvinceInsert(properties);
                if (!string.IsNullOrEmpty(sql))
                {
                    yield return sql;
                }
            }
            else if (IsMunicipality(properties))
            {
                var sql = BuildMunicipalityInsert(properties);
                if (!string.IsNullOrEmpty(sql))
                {
                    yield return sql;
                }
            }
        }
    }

    private static IEnumerable<JsonElement> ResolveFeatures(JsonElement root)
    {
        if (root.ValueKind == JsonValueKind.Array)
        {
            foreach (var element in root.EnumerateArray())
            {
                yield return element;
            }

            yield break;
        }

        if (root.TryGetProperty("features", out var featuresElement) &&
            featuresElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var element in featuresElement.EnumerateArray())
            {
                yield return element;
            }
        }
    }

    private static bool IsProvince(JsonElement properties)
    {
        return HasAny(properties, ProvinceCodeKeys) && HasAny(properties, ProvinceNameKeys);
    }

    private static bool IsMunicipality(JsonElement properties)
    {
        return HasAny(properties, MunicipalityCodeKeys) && HasAny(properties, MunicipalityNameKeys);
    }

    private static bool HasAny(JsonElement element, IEnumerable<string> keys)
    {
        foreach (var key in keys)
        {
            if (element.TryGetProperty(key, out var value) && value.ValueKind != JsonValueKind.Null &&
                value.ValueKind != JsonValueKind.Undefined)
            {
                return true;
            }
        }

        return false;
    }

    private static string? BuildProvinceInsert(JsonElement properties)
    {
        var code = GetFirstString(properties, ProvinceCodeKeys);
        var name = GetFirstString(properties, ProvinceNameKeys);

        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        var capital = GetFirstString(properties, ProvinceCapitalKeys);
        var area = GetFirstDecimal(properties, ProvinceAreaKeys);

        var values = new List<string>
        {
            FormatString(code),
            FormatString(name),
            FormatString(capital),
            FormatDecimal(area)
        };

        var insert = new StringBuilder();
        insert.Append("INSERT INTO public.coverage_province (code, name, capital, area_p5000) ");
        insert.Append("SELECT ");
        insert.Append(string.Join(", ", values));
        insert.Append(" WHERE NOT EXISTS (SELECT 1 FROM public.coverage_province WHERE code = ");
        insert.Append(FormatString(code));
        insert.Append(");");

        return insert.ToString();
    }

    private static string? BuildMunicipalityInsert(JsonElement properties)
    {
        var code = GetFirstString(properties, MunicipalityCodeKeys);
        var name = GetFirstString(properties, MunicipalityNameKeys);

        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        var capital = GetFirstString(properties, MunicipalityCapitalKeys);
        var area = GetFirstDecimal(properties, MunicipalityAreaKeys);
        var comarcaCode = GetFirstString(properties, MunicipalityComarcaCodeKeys);
        var comarcaName = GetFirstString(properties, MunicipalityComarcaNameKeys);
        var comarcaCapital = GetFirstString(properties, MunicipalityComarcaCapitalKeys);
        var vegueriaCode = GetFirstString(properties, MunicipalityVegueriaCodeKeys);
        var vegueriaName = GetFirstString(properties, MunicipalityVegueriaNameKeys);
        var vegueriaCapital = GetFirstString(properties, MunicipalityVegueriaCapitalKeys);
        var provinceCode = GetFirstString(properties, ProvinceReferenceKeys);

        var formattedValues = new List<string>
        {
            FormatString(code),
            FormatString(name),
            FormatString(capital),
            FormatDecimal(area),
            FormatString(comarcaCode),
            FormatString(comarcaName),
            FormatString(comarcaCapital),
            FormatString(vegueriaCode),
            FormatString(vegueriaName),
            FormatString(vegueriaCapital),
            FormatProvinceReference(provinceCode)
        };

        var insert = new StringBuilder();
        insert.Append("INSERT INTO public.coverage_municipality (code, name, capital, area_m5000, comarca_code, comarca_name, comarca_capital, vegueria_code, vegueria_name, vegueria_capital, coverage_province_id) ");
        insert.Append("SELECT ");
        insert.Append(string.Join(", ", formattedValues));
        insert.Append(" WHERE NOT EXISTS (SELECT 1 FROM public.coverage_municipality WHERE code = ");
        insert.Append(FormatString(code));
        insert.Append(");");

        return insert.ToString();
    }

    private static string FormatProvinceReference(string? provinceCode)
    {
        if (string.IsNullOrWhiteSpace(provinceCode))
        {
            return "NULL";
        }

        return $"(SELECT id FROM public.coverage_province WHERE code = {FormatString(provinceCode)})";
    }

    private static string FormatString(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "NULL";
        }

        var escaped = value.Replace("'", "''");
        return $"'{escaped}'";
    }

    private static string FormatDecimal(decimal? value)
    {
        if (value is null)
        {
            return "NULL";
        }

        return value.Value.ToString(CultureInfo.InvariantCulture);
    }

    private static string? GetFirstString(JsonElement element, IEnumerable<string> keys)
    {
        foreach (var key in keys)
        {
            if (element.TryGetProperty(key, out var property))
            {
                if (property.ValueKind == JsonValueKind.String)
                {
                    return property.GetString();
                }

                if (property.ValueKind == JsonValueKind.Number)
                {
                    return property.ToString();
                }
            }
        }

        return null;
    }

    private static decimal? GetFirstDecimal(JsonElement element, IEnumerable<string> keys)
    {
        foreach (var key in keys)
        {
            if (!element.TryGetProperty(key, out var property) || property.ValueKind != JsonValueKind.Number)
            {
                continue;
            }

            if (property.TryGetDecimal(out var value))
            {
                return value;
            }

            if (property.TryGetDouble(out var doubleValue))
            {
                return Convert.ToDecimal(doubleValue, CultureInfo.InvariantCulture);
            }
        }

        return null;
    }
}
