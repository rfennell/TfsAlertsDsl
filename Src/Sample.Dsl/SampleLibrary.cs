
namespace Sample.Dsl
{
    using System.ComponentModel.Composition;
    using TFSEventsProcessor.Dsl;
    using TFSEventsProcessor.Providers;

    /// <summary>
    /// Sample DSL Library
    /// This is used to show that MEF can load more than one DSL Library hence you can 
    /// logically separate libraries as needed
    /// </summary>
    [Export(typeof(IDslLibrary))]
    public class SampleLibrary : IDslLibrary
    {

        /// <summary>
        /// The link to TFS
        /// </summary>
        public ITfsProvider TfsProvider { get; set; }
        /// <summary>
        /// The link to email
        /// </summary>
        public IEmailProvider EmailProvider { get; set; }
        /// <summary>
        /// The raw XML of the event
        /// </summary>
        public string EventXml { get; set; }

        /// <summary>
        /// Simple add function
        /// </summary>
        /// <param name="p1">First variable</param>
        /// <param name="p2">Second variable</param>
        /// <returns>The sum of the parameters</returns>
        public string ScriptFolder { get; set; }

        public static int Add(int p1, int p2)
        {
            return p1 + p2;
        }

        /// <summary>
        /// Send an email 
        /// </summary>
        /// <param name="to">To address</param>
        /// <param name="subject">Email subject</param>
        /// <param name="body">Email body</param>
        public void SampleSendEmail(string to, string subject, string body)
        {
            this.EmailProvider.SendEmailAlert(to, subject, body);
        }
    }
}
