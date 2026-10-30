using CafePOS.DTO;

namespace CafePOS.GUI;

public class ChonBanForm : Form
{
    private readonly ListBox _listBox = new();

    public int? SelectedMaBan { get; private set; }

    public ChonBanForm(IReadOnlyList<BanAnDTO> tables)
    {
        Text = "Chọn bàn đích";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(360, 430);
        BackColor = UiTheme.Background;
        Font = UiTheme.BaseFont;

        var title = new Label
        {
            Text = "CHUYỂN BÀN",
            Dock = DockStyle.Top,
            Height = 50,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = UiTheme.TitleFont,
            ForeColor = UiTheme.Primary
        };

        _listBox.Dock = DockStyle.Fill;
        _listBox.Font = new Font("Segoe UI", 11F);
        _listBox.DisplayMember = nameof(BanAnDTO.TenBan);
        _listBox.DataSource = tables.ToList();
        _listBox.IntegralHeight = false;
        _listBox.ItemHeight = 30;

        var listPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(18, 0, 18, 10)
        };
        listPanel.Controls.Add(_listBox);

        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 60,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(12)
        };

        var cancelButton = UiTheme.CreateButton("Hủy", UiTheme.Muted, 100);
        cancelButton.DialogResult = DialogResult.Cancel;

        var okButton = UiTheme.CreateButton("Chuyển bàn", UiTheme.Primary, 130);
        okButton.Click += OkButton_Click;

        buttonPanel.Controls.Add(cancelButton);
        buttonPanel.Controls.Add(okButton);

        Controls.Add(listPanel);
        Controls.Add(buttonPanel);
        Controls.Add(title);

        AcceptButton = okButton;
        CancelButton = cancelButton;
    }

    private void OkButton_Click(object? sender, EventArgs e)
    {
        if (_listBox.SelectedItem is not BanAnDTO table)
        {
            MessageBox.Show(this, "Vui lòng chọn bàn đích.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        SelectedMaBan = table.MaBan;
        DialogResult = DialogResult.OK;
        Close();
    }
}
