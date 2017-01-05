namespace Sitecore.Support.ContentSearch.Client.Commands
{
    using System;
    using System.Collections.Specialized;
    using System.Threading;
    using Sitecore.Abstractions;
    using Sitecore.ContentSearch;
    using Sitecore.ContentSearch.Commands;
    using Sitecore.ContentSearch.Maintenance;
    using Sitecore.Diagnostics;
    using Sitecore.Jobs;
    using Sitecore.Shell.Applications.Dialogs.ProgressBoxes;
    using Sitecore.Shell.Framework.Commands;
    using Sitecore.Web.UI.Sheer;

    /// <summary>
    /// Represents the search:rebuild command.
    /// </summary>
    [Serializable]
    public class Rebuild : Command, IContentSearchCommand
    {
        /// <summary>
        /// The translate.
        /// </summary>
        [NonSerialized]
        private ITranslate translate;

        /// <summary>
        /// Gets the translation API.
        /// </summary>
        protected ITranslate Translate
        {
            get
            {
                if (this.translate == null)
                {
                    Interlocked.CompareExchange(
                        ref this.translate,
                        ContentSearchManager.Locator.GetInstance<ITranslate>(),
                        null);
                }

                return this.translate;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rebuild"/> class. 
        /// </summary>
        public Rebuild()
        {
            this.translate = ContentSearchManager.Locator.GetInstance<ITranslate>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rebuild"/> class.
        /// </summary>
        /// <param name="translate">
        /// The translate.
        /// </param>
        internal Rebuild(ITranslate translate)
        {
            this.translate = translate;
        }

        #region Public methods

        /// <summary>
        /// Executes the command in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void Execute([NotNull] CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            Assert.IsNotNull(context.Parameters, "parameters");
            Assert.IsTrue(context.Parameters.Count > 0, "parameters collection cannot be empty");

            // need this to prevent the Execute method from processed on post-back
            if (context.Items.Length > 0)
            {
                return;
            }

            var indexName = context.Parameters["index"];

            if (string.IsNullOrEmpty(indexName))
            {
                return;
            }

            var index = ContentSearchManager.GetIndex(indexName);

            if (index == null)
            {
                return;
            }

            Context.ClientPage.Start(this, "Run", new NameValueCollection { { "indexName", indexName } });
        }

        /// <summary>
        /// Runs the commend in ProgressBox.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        protected void Run(ClientPipelineArgs args)
        {
            string indexName = args.Parameters["indexName"];
            var title = string.Format("{0} ({1})", this.Translate.Text(Sitecore.ContentSearch.Localization.Texts.RebuildingIndex), indexName);
            var jobName = string.Format("{0} ({1})", this.Translate.Text(Sitecore.ContentSearch.Localization.Texts.IndexRebuild), indexName);
            ProgressBox.ExecuteSync(jobName, title, "People/16x16/hammer.png", this.RebuildIndex, this.RebuildDone);
        }

        /// <summary>
        /// The rebuild index.
        /// </summary>
        /// <param name="args">
        /// The Client Pipeline Arguments.
        /// </param>
        private void RebuildIndex(ClientPipelineArgs args)
        {
            string indexName = args.Parameters["indexName"];
            ISearchIndex index = ContentSearchManager.GetIndex(indexName);

            if (index == null)
            {
                return;
            }

            var uiJob = Context.Job;
            Assert.IsNotNull(uiJob, "UI Job");

            var clientLanguage = Context.Language;
            if (uiJob.Options != null)
            {
                clientLanguage = uiJob.Options.ClientLanguage;
            }

            var indexJob = IndexCustodian.FullRebuild(index);

            while (indexJob != null && !indexJob.IsDone)
            {
                var message = string.Format("{0}: {1}. ", this.Translate.TextByLanguage(Sitecore.ContentSearch.Localization.Texts.Status, clientLanguage), this.translate.TextByLanguage(indexJob.Status.State.ToString(), clientLanguage));

                if (indexJob.Status.State == JobState.Running)
                {
                    message += string.Format("{0}: {1}.", this.Translate.TextByLanguage(Sitecore.ContentSearch.Localization.Texts.Processed, clientLanguage), indexJob.Status.Processed);
                }

                uiJob.Status.Messages.Add(message);

                Thread.Sleep(500);
            }

            if (indexJob != null && indexJob.Status.Failed)
            {
                args.Parameters["failed"] = "1";
            }
        }

        /// <summary>
        /// Invoked when rebuild is finished.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        private void RebuildDone(ClientPipelineArgs args)
        {
            string message;
            if (args.Parameters["failed"] == "1")
            {
                message = this.Translate.Text(
                    Sitecore.ContentSearch.Localization.Texts.RebuildingIndexFailed,
                    args.Parameters["indexName"]);
            }
            else
            {
                message = this.Translate.Text(Sitecore.ContentSearch.Localization.Texts.IndexRebuilt, args.Parameters["indexName"]);
            }

            SheerResponse.Alert(message);
        }

        #endregion
    }
}