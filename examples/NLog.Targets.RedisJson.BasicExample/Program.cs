// See https://aka.ms/new-console-template for more information
using NLog;



Logger log = LogManager.GetCurrentClassLogger();
log.Error(new Exception(), "This is an error message");


Console.WriteLine("End");