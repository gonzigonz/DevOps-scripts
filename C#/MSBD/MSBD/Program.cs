using System;
using System.Windows.Forms;

namespace MSBD
{
    static class Program
    {
        private static Form1 _mainForm;

        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                RunGui();
            }
            else
            {
                RunAsConsole(args);
            }
        }

        private static void RunAsConsole(string[] args)
        {
            new MSBDConsole(args).RunWithArguments();
        }

        private static void RunGui()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            _mainForm = new Form1();
            Application.Run(_mainForm);
        }

        public static void Write(string message="")
        {
            _mainForm.WriteToTextBoxOutput(message);
        }

        public static void WriteLine(string message="")
        {
            _mainForm.WriteLineToTextBoxOutput(message);
        }

    }
}
