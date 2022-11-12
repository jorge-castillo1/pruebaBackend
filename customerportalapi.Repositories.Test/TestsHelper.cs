using System.IO;

namespace customerportalapi.Repositories.Test
{
    public class TestsHelper
    {
        public static string LoadJson(string path)
        {
            string json;
            using (StreamReader r = new StreamReader(path))
            { json = r.ReadToEnd(); }

            return json;
        }
    }
}
