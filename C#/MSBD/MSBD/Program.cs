using System;
using System.Windows.Forms;

namespace MSBD
{
    static class Program
    {
        private static Form1 _mainForm;

        [STAThread]
        static void Main()
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
