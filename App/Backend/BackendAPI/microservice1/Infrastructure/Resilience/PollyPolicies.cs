using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using System.Net;

namespace microservice1.Infrastructure.Resilience
{
    public static class PollyPolicies
    {
        // 1. Retry – handles transient failures
        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError() // 5xx, 408, HttpRequestException
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt =>
                        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // 2s, 4s, 8s
                    //onretry will be called before each retry - its useful for logging
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        // optional logging hook
                    });
        }

        // 2. Circuit Breaker – stops calling failing service
        public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    // below line means if 5 failures happen consecutively, the circuit will break
                    handledEventsAllowedBeforeBreaking: 5,
                    // after 30 seconds, try again
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    // onBreak and onReset are optional hooks for logging - they get called when circuit opens and closes
                    onBreak: (outcome, breakDelay) =>
                    {
                        // log circuit open
                    },
                    onReset: () =>
                    {
                        // log circuit reset
                    });
        }

        // 3. Timeout – cancels slow requests
        public static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy()
        {
            return Policy.TimeoutAsync<HttpResponseMessage>(
                // here we set 3 seconds as the timeout duration
                TimeSpan.FromSeconds(3),
                // we use pessimistic strategy which cancels the underlying task - in simpler terms, it aborts the HTTP request
                TimeoutStrategy.Pessimistic);
        }

        // 4. Bulkhead – limits concurrent calls
        public static IAsyncPolicy<HttpResponseMessage> GetBulkheadPolicy()
        {
            return Policy.BulkheadAsync<HttpResponseMessage>(
                // allow max 10 concurrent calls
                maxParallelization: 10,
                // allow max 20 calls to be queued
                maxQueuingActions: 20);
        }

        // 5. Fallback – graceful degradation
        public static IAsyncPolicy<HttpResponseMessage> GetFallbackPolicy()
        {
            return Policy<HttpResponseMessage>
                // here we handle both exceptions and unsuccessful HTTP responses
                .Handle<Exception>()
                // handle non-success status codes
                .OrResult(r => !r.IsSuccessStatusCode)
                // provide a fallback response
                .FallbackAsync(
                    // action to perform on fallback
                    fallbackAction: ct =>
                        // return a simple fallback response
                        Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StringContent("Fallback response from Service2")
                        }));
        }

        // 6. Wrap everything in correct order
        public static IAsyncPolicy<HttpResponseMessage> GetResiliencePolicy()
        {
            return Policy.WrapAsync(
                GetFallbackPolicy(),
                GetBulkheadPolicy(),
                GetRetryPolicy(),
                GetCircuitBreakerPolicy(),
                GetTimeoutPolicy());
        }
    }
}
