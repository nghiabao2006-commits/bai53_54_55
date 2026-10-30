namespace CafePOS;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.ThreadException += (_, e) =>
            MessageBox.Show(e.Exception.Message, "Lỗi không xử lý",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        Application.Run(new GUI.MainForm());
    }
}
