using static Data;

namespace ConverterSupport
{
    /// <summary>
    /// Code Page Class used for <see cref="CPS"/> Class (also see <see cref="AnsiCPMaps.AnsiCPMaps.CodePages"/>)
    /// </summary>
    /// <remarks></remarks>
    public class CodePage
    {
        private string myCode;
        private string myISO;

        private string myName;

        /// <summary>
        /// Constructor of <see cref="CodePage"/> Class
        /// </summary>
        /// <param name="Name">Name of Code Page</param>
        /// <param name="Code">Short Code/Abbreviation of Code Page</param>
        /// <param name="ISO">ISO Name of Code Page</param>
        /// <remarks></remarks>
        public CodePage(string Name, string Code, string ISO)
        {
            this.myCode = Code;
            this.myName = Name;
            this.myISO = ISO;
        }

        //New

        /// <summary>
        /// Returns Read-Only Property 'Code', Short Code/Abbreviation of Code Page
        /// </summary>
        /// <returns>String</returns>
        /// <remarks></remarks>
        public string Code
        {
            get { return myCode; }
        }

        /// <summary>
        /// Returns Name of Code Page
        /// </summary>
        /// <returns>String</returns>
        /// <remarks></remarks>
        public string Name
        {
            get { return myName; }
        }

        /// <summary>
        /// Returns ISO-Code of Code Page
        /// </summary>
        /// <returns>String</returns>
        /// <remarks></remarks>
        public string ISO
        {
            get { return myISO; }
        }
    }

    //CodePage
}