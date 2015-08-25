namespace SeleniumTest.Const
{
    public class Regex
    {
        public const string Image = @"\b(.png|.jpg|.gif)\b";
        public const string BackgroundImage = @"background-image:\s* url\(.*\b(.png|.jpg|.gif)\b.*\)";
        public const string BackgroundColor = @"background-color:\s* rgb\([0-9]{0,3}, \s*[0-9]{0,3}, \s*[0-9]{0,3}\);";
        public const string Url = @"(^http\://|/)[a-zA-Z0-9\-\.]+(\.[a-zA-Z]{2,3})?(/\S*)?$";
        public const string Link = @"(?i)<a([^>]+)>(.+?)</a>";
        public const string TextString = @"(?ix) \w+ \s*";
        public const string CopyRightString = @"^(?ix)©[0-9]{4}\s* .+ \s*$";
        public const string TextColor = @"^color:\s* rgb\([0-9]{0,3}, \s*[0-9]{0,3}, \s*[0-9]{0,3}\);";
        public const string Number = "@\"[0-9]*\"";
    }
}
