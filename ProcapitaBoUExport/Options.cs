using System;
using CommandLine;

namespace ProcapitaBoUExport
{
    class Options
    {
        [Option('s', "searchdate", HelpText = "Search date.")]
        public DateTime? SearchDate { get; set; }

        [Option('v', "verbose", DefaultValue = false,
            HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return CommandLine.Text.HelpText.AutoBuild(this,
              (CommandLine.Text.HelpText current) => CommandLine.Text.HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
