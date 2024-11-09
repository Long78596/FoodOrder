

namespace FoodOrder.Areas.Admin.Repository

{
    public class SensitiveWordFilter
    {
        private static readonly List<string> sensitiveWords = new List<string> { "hi", "hii", "hiii" };

        public bool ContainsSensitiveWords(string comment)
        {
            return sensitiveWords.Any(word => comment.Contains(word, StringComparison.OrdinalIgnoreCase));
        }
        public bool ExceedsWordLimit(string comment, int limit = 10)
        {
            var wordCount = comment?.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length ?? 0;
            return wordCount > limit;
        }
    }
}
