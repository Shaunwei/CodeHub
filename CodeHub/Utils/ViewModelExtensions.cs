using System;
using System.Threading.Tasks;
using CodeFramework.ViewModels;
using GitHubSharp;
using System.Collections.Generic;

namespace CodeHub.ViewModels
{
    public static class ViewModelExtensions
    {
        public static void RequestModel<TRequest>(this ViewModel viewModel, GitHubRequest<TRequest> request, bool forceDataRefresh, System.Action<GitHubSharp.GitHubResponse<TRequest>> update) where TRequest : new()
        {
            var stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();

            if (forceDataRefresh)
            {
                request.CheckIfModified = false;
                request.RequestFromCache = false;
            }

            var response = Application.Client.Execute(request);
            stopWatch.Stop();

            Console.WriteLine("Request executed in: " + stopWatch.ElapsedMilliseconds + "ms");

            stopWatch.Reset();
            stopWatch.Start();
            update(response);
            stopWatch.Stop();

            Console.WriteLine("View updated in: " + stopWatch.ElapsedMilliseconds + "ms");


            if (response.WasCached)
            {
                System.Threading.Tasks.Task.Run(() => {
                    try
                    {
                        request.RequestFromCache = false;
                        update(Application.Client.Execute(request));
                    }
                    catch (GitHubSharp.NotModifiedException)
                    {
                        Console.WriteLine("Not modified: " + request.Url);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("SHIT! " + request.Url);
                    }
                });
            }
        }

        public static void CreateMore<T>(this ViewModel viewModel, 
                                         GitHubResponse<T> response, 
                                         Action<Action> assignMore, 
                                         Action<T> newDataAction) where T : new()
        {
            if (response.More == null)
            {
                assignMore(null);
                return;
            }

            assignMore(new Action(() => {
                response.More.UseCache = false;
                var moreResponse = Application.Client.Execute(response.More);
                viewModel.CreateMore(moreResponse, assignMore, newDataAction);
                newDataAction(moreResponse.Data);
            }));
        }

        public static Task SimpleCollectionLoad<T>(this CollectionViewModel<T> viewModel, GitHubRequest<List<T>> request, bool forceDataRefresh) where T : new()
        {
            return Task.Run(() => viewModel.RequestModel(request, forceDataRefresh, response => {
                viewModel.Items.Reset(response.Data);
                viewModel.CreateMore(response, m => viewModel.MoreItems = m, d => viewModel.Items.AddRange(d));
            }));
        }
    }
}

