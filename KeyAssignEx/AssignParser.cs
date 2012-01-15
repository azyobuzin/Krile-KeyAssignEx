using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace KeyAssignEx
{
    static class AssignParser
    {
        public static XElement Parse(string path)
        {
            return XElement.Parse(
                File.ReadAllLines(path)
                    .Select(line => line.Contains("//")
                        ? line.Substring(0,
                            line.EnumerateIndex("//")
                                .Where(index => (line.Take(index).Count(c => c == '"' || c == '\'') % 2) == 0)
                                .Select(index => index + 1)
                                .FirstOrDefault()
                                .Case(0, line.Length + 1)
                                - 1
                          )
                        : line
                    )
                    .Join("\r\n")
            );
        }
    }
}
