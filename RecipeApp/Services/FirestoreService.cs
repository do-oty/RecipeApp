using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace RecipeApp.Services
{
    public class FirestoreService
    {
        private readonly HttpClient _httpClient;
        private readonly string _projectId;
        private readonly string _apiKey;
        private const string FirestoreBaseUrl = "https://firestore.googleapis.com/v1/projects/";

        public FirestoreService()
        {
            _httpClient = new HttpClient();
            _projectId = "recipeapp-2b12e"; // Your Firebase project ID
            _apiKey = "AIzaSyCzG5FTSuD2lHB_RAoEM2WMjk1LihO_GQ4"; // Your Firebase API key
        }

        public async Task<T> GetDocumentAsync<T>(string collection, string documentId)
        {
            try
            {
                var url = $"{FirestoreBaseUrl}{_projectId}/databases/(default)/documents/{collection}/{documentId}?key={_apiKey}";
                var response = await _httpClient.GetAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var document = JsonConvert.DeserializeObject<FirestoreDocument>(content);
                    if (document?.Fields != null)
                    {
                        var json = JsonConvert.SerializeObject(document.Fields.ToDictionary(kv => kv.Key, kv => kv.Value.StringValue));
                        return JsonConvert.DeserializeObject<T>(json);
                    }
                }
                
                return default;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Firestore Error: {ex.Message}");
                return default;
            }
        }

        private Dictionary<string, object> ToFirestoreFields(object data)
        {
            var dict = new Dictionary<string, object>();
            foreach (var prop in data.GetType().GetProperties())
            {
                var value = prop.GetValue(data);
                if (value is string s)
                    dict[prop.Name] = new Dictionary<string, object> { { "stringValue", s } };
                else if (value is DateTime dt)
                    dict[prop.Name] = new Dictionary<string, object> { { "stringValue", dt.ToString("o") } };
                // Add more types as needed (int, bool, etc.)
            }
            return dict;
        }

        public async Task<bool> CreateDocumentAsync<T>(string collection, string documentId, T data)
        {
            System.Diagnostics.Debug.WriteLine($"Firestore CreateDocumentAsync: collection={collection}, documentId={documentId}");
            if (string.IsNullOrWhiteSpace(documentId))
            {
                System.Diagnostics.Debug.WriteLine("Firestore CreateDocumentAsync Error: documentId is null or empty.");
                return false;
            }
            try
            {
                var url = $"{FirestoreBaseUrl}{_projectId}/databases/(default)/documents/{collection}/{documentId}?key={_apiKey}";
                var document = new Dictionary<string, object>
                {
                    { "fields", ToFirestoreFields(data) }
                };
                var content = new StringContent(JsonConvert.SerializeObject(document), Encoding.UTF8, "application/json");
                var response = await _httpClient.PatchAsync(url, content);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Firestore CreateDocumentAsync Error: {error}");
                }
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Firestore Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateDocumentAsync<T>(string collection, string documentId, T data)
        {
            if (string.IsNullOrWhiteSpace(documentId))
            {
                System.Diagnostics.Debug.WriteLine("Firestore UpdateDocumentAsync Error: documentId is null or empty.");
                return false;
            }
            try
            {
                var url = $"{FirestoreBaseUrl}{_projectId}/databases/(default)/documents/{collection}/{documentId}?key={_apiKey}";
                var document = new Dictionary<string, object>
                {
                    { "fields", ToFirestoreFields(data) }
                };
                var content = new StringContent(JsonConvert.SerializeObject(document), Encoding.UTF8, "application/json");
                var response = await _httpClient.PatchAsync(url, content);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Firestore UpdateDocumentAsync Error: {error}");
                }
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Firestore Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteDocumentAsync(string collection, string documentId)
        {
            try
            {
                var url = $"{FirestoreBaseUrl}{_projectId}/databases/(default)/documents/{collection}/{documentId}?key={_apiKey}";
                var response = await _httpClient.DeleteAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Firestore DeleteDocumentAsync Error: {error}");
                }
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Firestore Error: {ex.Message}");
                return false;
            }
        }

        public async Task<List<string>> QueryFavoriteRecipeIdsAsync(string collection, string userId)
        {
            try
            {
                var url = $"{FirestoreBaseUrl}{_projectId}/databases/(default)/documents:runQuery?key={_apiKey}";
                var query = new
                {
                    structuredQuery = new
                    {
                        from = new[] { new { collectionId = collection } },
                        where = new
                        {
                            fieldFilter = new
                            {
                                field = new { fieldPath = "UserId" },
                                op = "EQUAL",
                                value = new { stringValue = userId }
                            }
                        }
                    }
                };

                var content = new StringContent(JsonConvert.SerializeObject(query), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var documents = JsonConvert.DeserializeObject<List<FirestoreQueryResult>>(responseContent);
                    return documents
                        .Where(d => d.Document?.Fields != null && d.Document.Fields.ContainsKey("RecipeId"))
                        .Select(d => d.Document.Fields["RecipeId"].StringValue)
                        .ToList();
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Firestore QueryDocumentsAsync Error: {error}");
                }

                return new List<string>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Firestore Error: {ex.Message}");
                return new List<string>();
            }
        }
    }

    public class FirestoreField
    {
        [Newtonsoft.Json.JsonProperty("stringValue")]
        public string StringValue { get; set; }
    }

    public class FirestoreDocument
    {
        [Newtonsoft.Json.JsonProperty("fields")]
        public Dictionary<string, FirestoreField> Fields { get; set; }
    }

    public class FirestoreQueryResult
    {
        [Newtonsoft.Json.JsonProperty("document")]
        public FirestoreDocument Document { get; set; }
    }
} 