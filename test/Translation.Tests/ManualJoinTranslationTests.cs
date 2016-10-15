using System;
using System.Linq;
using Xunit;
using Translation;
using Translation.DbObjects.SqlObjects;
using Translation.EF;

namespace Translation.Tests
{
    public class ManualTranslationTests
    {
        [Fact]
        public void Test_Translate_Join_Select_Columns() 
        {
            using (var db = new TestingContext())
            {
                var query = db.Blogs.Where(b => b.Posts.Any(p => p.User.UserName != null));
                var query1 = db.Posts.
                    Join(
                        query, 
                        (p, b) => p.BlogId == b.BlogId && p.User.UserName == "ethan", 
                        (p, b) => new { PId = p.PostId, b.Name },
                        JoinType.LeftOuter);

                var script = LinqTranslator.Translate(query1.Expression, new EFModelInfoProvider(db), new SqlObjectFactory());
                var sql = script.ToString();

                const string expected = @"
select sq0.*
from (
    select p0.'PostId' as 'PId', sq0.'Name' as 'Name'
    from Posts p0
    inner join Users u0 on p0.'UserId' = u0.'UserId'
    left outer join (
        select b0.'Name', b0.'BlogId' as 'BlogId_jk1'
        from Blogs b0
        left outer join (
            select p0.'BlogId' as 'BlogId_jk0'
            from Posts p0
            inner join Users u0 on p0.'UserId' = u0.'UserId'
            where u0.'UserName' is not null
            group by p0.'BlogId'
        ) sq0 on b0.'BlogId' = sq0.'BlogId_jk0'
        where sq0.'BlogId_jk0' is not null
    ) sq0 on p0.'BlogId' = sq0.'BlogId_jk1' and u0.'UserName' = 'ethan'
) sq0
";

                TestUtils.AssertStringEqual(expected, sql);                
            }
        }
    }
}
