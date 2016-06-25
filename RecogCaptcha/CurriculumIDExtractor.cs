using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Lattestrac.Lib
{
    public static class CurriculumIDExtractor
    {
        public static List<string> GetPageCurriculums(string pageHTML)
        {
            List<string> returnIDs = new List<string>();

            Regex curriculumExpression = new Regex(@"abreDetalhe\('(.*?)'");

            var results = curriculumExpression.Matches(pageHTML);

            foreach (Match m in results)
            {
                returnIDs.Add(m.Groups[1].Value);
            }

            return returnIDs;
        }
    }
}
