namespace Sitecore.Support.ContentSearch.Client.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
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
    /// Represents the indexing:rebuildall command.
    /// </summary>
    [Serializable]
    public class RebuildAll : Command, IContentSearchCommand
    {
        /// <summary>
        /// The translate.
        /// </summary>
        [NonSerialized]
        private ITranslate translate;

        public ITranslate Translate
        {
            get
            {
                return this.translate ?? ContentSearchManager.Locator.GetInstance<ITranslate>();
            }
            set
            {
                this.translate = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RebuildAll"/> class.
        /// </summary>
        public RebuildAll()
        {
            this.Translate = ContentSearchManager.Locator.GetInstance<ITranslate>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RebuildAll"/> class.
        /// </summary>
        /// <param name="translate">
        /// The translate.
        /// </param>
        internal RebuildAll(ITranslate translate)
        {
            this.Translate = translate;
        }

        #region Public methods

        /// <summary>
        /// Executes the command in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void Execute([NotNull] CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            Context.ClientPage.Start(this, "Run", new NameValueCollection());
        }

        /// <summary>
        /// Runs the command in ProgressBox.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        protected void Run(ClientPipelineArgs args)
        {
            var jobName = string.Format("{0}", this.Translate.Text(Sitecore.ContentSearch.Localization.Texts.RebuildingAllIndexes));
            var headerText = this.Translate.Text(Sitecore.ContentSearch.Localization.Texts.RebuildingAllIndexes);
            ProgressBox.ExecuteSync(jobName, headerText, "People/16x16/hammer.png", this.RebuildAllIndexes, this.RebuildDone);
        }

        /// <summary>
        /// The rebuild all indexes.
        /// </summary>
        /// <param name="args">
        /// The Client Pipeline Args.
        /// </param>
        private void RebuildAllIndexes(ClientPipelineArgs args)
        {
            List<Job> jobs = IndexCustodian.RebuildAll(new[] { IndexGroup.Experience }).ToList();

            while (jobs.Any(j => !j.IsDone))
            {
                Thread.Sleep(500);
            }

            int failed = jobs.Count(j => j.Status.Failed);

            if (failed > 0)
            {
                args.Parameters["failed"] = "1";
                args.Parameters["failedNumber"] = failed.ToString();
                args.Parameters["totalNumber"] = jobs.Count.ToString();
            }
        }

        /// <summary>
        /// Invoked when rebuild is done.
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
                    Sitecore.ContentSearch.Localization.Texts.RebuildingAllFailed,
                    args.Parameters["failedNumber"],
                    args.Parameters["totalNumber"]);
            }
            else
            {
                message = this.Translate.Text(Sitecore.ContentSearch.Localization.Texts.AllIndexesRebuilt);
            }

            SheerResponse.Alert(message);
        }

        #endregion
    }
}