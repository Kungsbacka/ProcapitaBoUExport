using System;
using System.Collections.Generic;
using CommandLine;

namespace ProcapitaBoUExport
{
    class Options
    {
        [Option('s', "searchdate", HelpText = "Search date.")]
        public DateTime? SearchDate { get; set; }

        [Option('v', "verbose", Default = false, HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }

        [Option('u', "singleUnit", SetName = "single", HelpText = "Unit name. Fetch only this unit.")]
        public string SingleUnit { get; set; }

        [Option('m', "multipleUnits", SetName = "multiple", HelpText = "List of unit names. Fetch only units in list.")]
        public IEnumerable<string> Units { get; set; }
    }
}
