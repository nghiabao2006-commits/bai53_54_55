namespace InventoryManagement;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.ThreadException += (_, e) =>
            MessageBox.Show(e.Exception.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

        try
        {
            DAL.DbHelper.Initialize();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Không thể chuẩn bị cơ sở dữ liệu.\n\n" + ex.Message,
                "Lỗi cơ sở dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        Application.Run(new GUI.MainForm());
    }
}
