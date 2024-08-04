using RestApp.Model;

namespace RestApp.Repository
{
    public class RestClient : IRestClient
    {
        private readonly HttpClient _httpClient;

        public RestClient()
        {
            _httpClient = new HttpClient();
        }

        public Task<TModel> Delete<TModel>(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<string> Get(string url)
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public Task<TModel> Get<TModel>(string url)
        {
            throw new NotImplementedException();
        }

        public Task<TModel> Post<TModel>(string url, TModel model)
        {
            throw new NotImplementedException();
        }

        public Task<TModel> Put<TModel>(string url, TModel model)
        {
            throw new NotImplementedException();
        }
    }
}