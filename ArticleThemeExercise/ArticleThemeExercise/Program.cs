using System.Text.Json;

using (StreamReader themeReader = new StreamReader(args[0]))
{
    using (StreamReader articleReader = new StreamReader(args[1]))
    {
        List<int> articleIndices = new List<int>();
        string articleJSON = await articleReader.ReadToEndAsync();
        string themeJSON = await themeReader.ReadToEndAsync();
        ArticleContainer? articleContainer = JsonSerializer.Deserialize<ArticleContainer>(articleJSON);
        ThemeContainer? themeContainer = JsonSerializer.Deserialize<ThemeContainer>(themeJSON);

        for (int t = 0; t < themeContainer.data.Count; t++)
        {
            Theme currentTheme = themeContainer.data[t];
            Console.WriteLine($"\nTHEME {currentTheme.id} : {currentTheme.title}");
            Console.WriteLine("Please, select an article : ");
            for (int a = 0; a < currentTheme.articleIds.Count; a++)
            {
                Console.WriteLine($"{a + 1}. {articleContainer.data[currentTheme.articleIds[a]].content}");
            }

            while(true)
            {
                string? articleID = Console.ReadLine();
                try
                {
                    int id = int.Parse(articleID);
                    if(id > 0 && id <= currentTheme.articleIds.Count)
                    {
                        articleIndices.Add(currentTheme.articleIds[id - 1]);
                        break;
                    }

                    Console.WriteLine("[-] Index Absent");
                }
                catch{ Console.WriteLine("[-] Invalid Input, Please Insert Digits"); }
            }
        }

        float effectSum = 0f;
        string printableIDs = "";
        for (int i = 0; i < articleIndices.Count; i++)
        {
            Article currentArticle = articleContainer.data[articleIndices[i]];
            effectSum += currentArticle.id % 2 == 0 ? currentArticle.effect * 2 : currentArticle.effect / 2f;
            printableIDs += i == 0 ? articleIndices[i].ToString() : $" > {articleIndices[i]}";
        }

        Console.WriteLine($"\nSUM : {effectSum}, IDs : {printableIDs}");
    }
}

public class ThemeContainer
{
    public List<Theme> data { get; set; }
}

public class Theme
{
    public int id { get; set; }
    public string? title { get; set; }
    public List<int> articleIds { get; set; }
}
public class ArticleContainer
{
    public List<Article> data { get; set; }
}
public class Article
{
    public int id { get; set; }
    public string? content { get; set; }
    public int effect { get; set; }
}