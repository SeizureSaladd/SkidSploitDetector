public class gist_integration
    {
	  /* Nihon and vivian :weary:, vector sucks. Credits to null */

        public string token;
        public GitHubClient client;

        public gist_integration(string _token)
        {
            token = _token;

            client = new GitHubClient(new ProductHeaderValue("test-app"))
            {
                Credentials = new Credentials(token)
            };
        }

        public KeyValuePair<string, GistFile> get_gist(string id) =>
          client.Gist.Get(id).Result.Files.FirstOrDefault();

        public IReadOnlyList<Gist> get_allgist() =>
            client.Gist.GetAll().GetAwaiter().GetResult();

        public IReadOnlyList<Gist> get_allgist(string user) =>
            client.Gist.GetAllForUser(user).GetAwaiter().GetResult();

        public void upload_gist(
            string name, string content, string desc, bool _public = false)
        {
            var gist = new NewGist()
            {
                Description = desc,
                Public = _public,
            };

            gist.Files.Add(name, content);
            Process.Start(client.Gist.Create(gist).GetAwaiter().GetResult().HtmlUrl);
            Console.WriteLine($"gist created, title : {name} content : {content}");
        }

        public void delete_gist(string id)
        {
            client.Gist.Delete(id);
            Console.WriteLine($"gist deleted : {id}");
        }

        public void edit_gist(string new_file_name, string new_content)
        {
            var updateGist = new GistUpdate
            {
                Description = "my newly updated gist"
            };
            var gistFileUpdate = new GistFileUpdate
            {
                NewFileName = new_file_name,
                Content = new_content
            };

            updateGist.Files.Add("myGistTestFile.cs", gistFileUpdate);

            client.Gist.Edit("1", updateGist);
        }
    }
