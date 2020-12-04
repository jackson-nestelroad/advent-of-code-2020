using AdventOfCode2020.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2020.Days
{
    [Solution(4)]
    class Day04 : ISolution
    {
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
        private class PassportEntryAttribute : Attribute
        {
            public bool Required { get; }
            public Regex Pattern { get; }

            public PassportEntryAttribute(string pattern, bool required = true)
            {
                Pattern = new Regex(pattern);
                Required = required;
            }
        }

        private class Passport
        {
            private readonly Dictionary<string, string> Entries = new Dictionary<string, string>();

            [PassportEntry(@"^(19[2-9][0-9]|200[0-2])$")]
            public string BirthYear
            {
                get => Entries.GetValue("byr");
            }
            [PassportEntry(@"^20(1[0-9]|20)$")]
            public string IssueYear
            {
                get => Entries.GetValue("iyr");
            }
            [PassportEntry(@"^20(2[0-9]|30)$")]
            public string ExpirationYear
            {
                get => Entries.GetValue("eyr");
            }
            [PassportEntry(@"^((1([5-8][0-9]|9[0-3])cm)|((59|6[0-9]|7[0-6])in))$")]
            public string Height
            {
                get => Entries.GetValue("hgt");
            }
            [PassportEntry(@"^#[a-f0-9]{6}$")]
            public string HairColor
            {
                get => Entries.GetValue("hcl");
            }
            [PassportEntry("^(amb|blu|brn|gry|grn|hzl|oth)$")]
            public string EyeColor
            {
                get => Entries.GetValue("ecl");
            }
            [PassportEntry("^[0-9]{9}$")]
            public string PasswordId
            {
                get => Entries.GetValue("pid");
            }
            [PassportEntry(".*", false)]
            public string CountryId
            {
                get => Entries.GetValue("cid");
            }

            public void AddField(string key, string value) => Entries.Add(key, value);

            private IEnumerable<(System.Reflection.PropertyInfo Property, PassportEntryAttribute Attributes)>
                GetPropertiesAndAttributes()
            {
                return GetType()
                    .GetProperties()
                    .Select(prop => (Property: prop, Attributes:
                        (PassportEntryAttribute)Attribute.GetCustomAttribute(prop, typeof(PassportEntryAttribute))))
                    .Where(tuple => tuple.Attributes != null);
            }

            public bool HasAllRequiredFields()
            {
                foreach (var (Property, Attributes) in GetPropertiesAndAttributes())
                {
                    if (Attributes.Required && Property.GetValue(this) == null)
                    {
                        return false;
                    }
                }
                return true;
            }

            public bool IsValid()
            {
                foreach (var (Property, Attributes) in GetPropertiesAndAttributes())
                {
                    if (Attributes.Required)
                    {
                        if (Property.GetValue(this) is not string value || !Attributes.Pattern.IsMatch(value))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }
        private static List<Passport> ParseInput(string input)
        {
            return input.Split(new string[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.None).Select(data =>
            {
                Passport passport = new Passport();
                foreach (string field in data.TrimEnd().Split())
                {
                    int delimPos = field.IndexOf(':');
                    if (delimPos < 0)
                    {
                        throw new InputParseException();
                    }
                    passport.AddField(field[0..delimPos], field[(delimPos + 1)..]);
                }
                return passport;
            }).ToList();
        }

        public object PartA(string input)
        {
            List<Passport> passports = ParseInput(input);
            return passports.Count(passport => passport.HasAllRequiredFields());
        }

        public object PartB(string input)
        {
            List<Passport> passports = ParseInput(input);
            return passports.Count(Passport => Passport.IsValid());
        }
    }
}
