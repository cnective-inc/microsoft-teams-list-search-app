﻿// <copyright file="QnAMakerService.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
namespace Lib.Helpers
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Lib.Models;
    using Newtonsoft.Json;

    /// <summary>
    /// Helper for accessing QnA Maker APIs
    /// </summary>
    public class QnAMakerService : IQnAMakerService
    {
        /// <summary>
        /// QnA Maker Request url
        /// </summary>
        private const string QnAMakerRequestUrl = "https://westus.api.cognitive.microsoft.com/qnamaker/v4.0";

        private const string MethodKB = "knowledgebases";

        private const string MethodOperation = "operations";

        /// <summary>
        /// Host url of the compute application
        /// </summary>
        private readonly string hostUrl;

        /// <summary>
        /// Id of KB to be queried.
        /// </summary>
        private readonly string kbId;

        /// <summary>
        /// Endpoint key for the published Kb to be searched.
        /// </summary>
        private readonly string endpointKey;

        /// <summary>
        /// Ocp-Apim-Subscription-Key for the QnA Maker service
        /// </summary>
        private readonly string subscriptionKey;

        /// <summary>
        /// Http client for generating http requests.
        /// </summary>
        private readonly HttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="QnAMakerService"/> class.
        /// This constructor initializes an instance meant for GenerateAnswerAsync method.
        /// </summary>
        /// <param name="hostUrl">Host url of the compute application</param>
        /// <param name="kbId">Id of the KB to be queried</param>
        /// <param name="endpointKey">Endpoint key for the published kb to be searched</param>
        /// <param name="httpClient">HttpClient for generating http requests</param>
        public QnAMakerService(string hostUrl, string kbId, string endpointKey, HttpClient httpClient)
        {
            this.hostUrl = hostUrl;
            this.kbId = kbId;
            this.endpointKey = endpointKey;
            this.httpClient = httpClient;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QnAMakerService"/> class.
        /// This constructr initializes an instance meant for Update, Publish and GetOperation APIs.
        /// </summary>
        /// <param name="kbId">Id of the KB to be queried</param>
        /// <param name="subscriptionKey">Ocp-Apim-Subscription-Key for the QnA Maker service</param>
        /// <param name="httpClient">Http Client to be used.</param>
        public QnAMakerService(string kbId, string subscriptionKey, HttpClient httpClient)
        {
            this.kbId = kbId;
            this.subscriptionKey = subscriptionKey;
            this.httpClient = httpClient;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QnAMakerService"/> class.
        /// This constructr initializes an instance meant for Create API.
        /// </summary>
        /// <param name="subscriptionKey">Ocp-Apim-Subscription-Key for the QnA Maker service</param>
        /// <param name="httpClient">Http Client to be used.</param>
        public QnAMakerService(string subscriptionKey, HttpClient httpClient)
        {
            this.subscriptionKey = subscriptionKey;
            this.httpClient = httpClient;
        }

        /// <inheritdoc/>
        public async Task<GenerateAnswerResponse> GenerateAnswerAsync(GenerateAnswerRequest request)
        {
            try
            {
                string service = "/qnamaker";
                string method = "/knowledgebases/" + this.kbId + "/generateAnswer/";
                string uri = this.hostUrl + service + method;
                using (HttpRequestMessage httpRequest = new HttpRequestMessage())
                {
                    httpRequest.Method = HttpMethod.Post;
                    httpRequest.RequestUri = new Uri(uri);
                    httpRequest.Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                    httpRequest.Headers.Add("Authorization", "EndpointKey " + this.endpointKey);

                    HttpResponseMessage response = await this.httpClient.SendAsync(httpRequest);
                    if (response.IsSuccessStatusCode)
                    {
                        return JsonConvert.DeserializeObject<GenerateAnswerResponse>(await response.Content.ReadAsStringAsync());
                    }
                    else
                    {
                        throw new Exception($"HTTP Error code - {response.StatusCode} with reason phrase {response.ReasonPhrase}");
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<QnAMakerResponse> UpdateKB(UpdateKBRequest body)
        {
            try
            {
                this.httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", this.subscriptionKey);
                string uri = $"{QnAMakerRequestUrl}/{MethodKB}/{this.kbId}";

                using (HttpRequestMessage httpRequest = new HttpRequestMessage())
                {
                    httpRequest.Method = new HttpMethod("PATCH");
                    httpRequest.RequestUri = new Uri(uri);
                    httpRequest.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await this.httpClient.SendAsync(httpRequest);
                    if (response.IsSuccessStatusCode)
                    {
                        return JsonConvert.DeserializeObject<QnAMakerResponse>(await response.Content.ReadAsStringAsync());
                    }
                    else
                    {
                        throw new Exception($"HTTP Error code - {response.StatusCode} with reason phrase {response.ReasonPhrase}");
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<QnAMakerResponse> PublishKB()
        {
            try
            {
                this.httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", this.subscriptionKey);
                var uri = $"{QnAMakerRequestUrl}/{MethodKB}/{this.kbId}";
                HttpResponseMessage response;
                byte[] byteData = Encoding.UTF8.GetBytes("{body}");
                using (var content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    response = await this.httpClient.PostAsync(uri, content);
                    if (response.IsSuccessStatusCode)
                    {
                        return JsonConvert.DeserializeObject<QnAMakerResponse>(await response.Content.ReadAsStringAsync());
                    }
                    else
                    {
                        throw new Exception($"HTTP Error code - {response.StatusCode} with reason phrase {response.ReasonPhrase}");
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<QnAMakerResponse> CreateKB(CreateKBRequest body)
        {
            try
            {
                this.httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", this.subscriptionKey);
                var uri = $"{QnAMakerRequestUrl}/{MethodKB}/create";

                using (HttpRequestMessage httpRequest = new HttpRequestMessage())
                {
                    httpRequest.Method = new HttpMethod("POST");
                    httpRequest.RequestUri = new Uri(uri);
                    httpRequest.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await this.httpClient.SendAsync(httpRequest);
                    if (response.IsSuccessStatusCode)
                    {
                        return JsonConvert.DeserializeObject<QnAMakerResponse>(await response.Content.ReadAsStringAsync());
                    }
                    else
                    {
                        throw new Exception($"HTTP Error code - {response.StatusCode} with reason phrase {response.ReasonPhrase}");
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<QnAMakerResponse> GetOperationDetails(string operationId)
        {
            try
            {
                this.httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", this.subscriptionKey);
                var uri = $"{QnAMakerRequestUrl}/{MethodOperation}/{operationId}";
                var response = await this.httpClient.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<QnAMakerResponse>(await response.Content.ReadAsStringAsync());
                }
                else
                {
                    throw new Exception($"HTTP Error code - {response.StatusCode} with reason phrase {response.ReasonPhrase}");
                }
            }
            catch
            {
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<GetKnowledgeBaseDetailsResponse> GetKnowledgeBaseDetails()
        {
            try
            {
                this.httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", this.subscriptionKey);
                var uri = $"{QnAMakerRequestUrl}/{MethodKB}/{this.kbId}";
                var response = await this.httpClient.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<GetKnowledgeBaseDetailsResponse>(await response.Content.ReadAsStringAsync());
                }
                else
                {
                    throw new Exception($"HTTP Error code - {response.StatusCode} with reason phrase {response.ReasonPhrase}");
                }
            }
            catch
            {
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<string> AwaitOperationCompletionState(QnAMakerResponse response)
        {
            int delay = 1000; // ms
            int count = 0;
            QnAMakerResponse getOperationDetailsResponse = response;
            while (!this.IsOperationComplete(getOperationDetailsResponse))
            {
                // limit of 3 qps.
                if (count == 3)
                {
                    await Task.Delay(delay);
                    count = 0;
                }

                getOperationDetailsResponse = await this.GetOperationDetails(response.OperationId);
                count++;
            }

            return getOperationDetailsResponse.OperationState;
        }

        /// <inheritdoc/>
        public bool IsOperationSuccessful(string operationState)
        {
            if (operationState == QnAMakerOperationStates.Succeeded)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if operation is completed.
        /// </summary>
        /// <param name="response">Response to be checked if completed.</param>
        /// <returns><see cref="bool"/> that represents if operation is complete.</returns>
        private bool IsOperationComplete(QnAMakerResponse response)
        {
            if (response?.OperationState == QnAMakerOperationStates.Succeeded)
            {
                return true;
            }
            else if (response?.OperationState == QnAMakerOperationStates.Running || response?.OperationState == QnAMakerOperationStates.NotStarted)
            {
                return false;
            }
            else
            {
                StringBuilder details = new StringBuilder();
                foreach (var detail in response.ErrorResponse.Error.Details)
                {
                    details.Append(detail + Environment.NewLine);
                }

                throw new Exception($"Error Code: {response.ErrorResponse.Error.Code} {Environment.NewLine} Error Message: {response.ErrorResponse.Error.Message} {Environment.NewLine} Error Details: {details.ToString()}");
            }
        }
    }
}
