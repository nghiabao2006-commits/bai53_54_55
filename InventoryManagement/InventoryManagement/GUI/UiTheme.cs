namespace InventoryManagement.GUI;

internal static class UiTheme
{
    public static readonly Color Background = Color.FromArgb(245, 247, 250);
    public static readonly Color Surface = Color.White;
    public static readonly Color Primary = Color.FromArgb(0, 105, 92);
    public static readonly Color PrimaryDark = Color.FromArgb(0, 77, 64);
    public static readonly Color Accent = Color.FromArgb(230, 81, 0);
    public static readonly Color Success = Color.FromArgb(46, 125, 50);
    public static readonly Color Danger = Color.FromArgb(198, 40, 40);
    public static readonly Color Warning = Color.FromArgb(245, 124, 0);
    public static readonly Color Muted = Color.FromArgb(110, 115, 120);
    public static readonly Color Line = Color.FromArgb(224, 228, 235);

    public static readonly Font BaseFont = new("Segoe UI", 10F);
    public static readonly Font SmallFont = new("Segoe UI", 9F);
    public static readonly Font BoldFont = new("Segoe UI", 10F, FontStyle.Bold);
    public static readonly Font TitleFont = new("Segoe UI", 13F, FontStyle.Bold);

    public static Button CreateButton(string text, Color background, int width = 118, int height = 36)
    {
        var button = new Button
        {
            Text = text,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            MinimumSize = new Size(width, height),
            Padding = new Padding(8, 0, 8, 0),
            BackColor = background,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = BoldFont,
            Cursor = Cursors.Hand,
            Margin = new Padding(4, 5, 4, 3),
            UseVisualStyleBackColor = false
        };
        button.FlatAppearance.BorderSize = 0;
        return button;
    }

    public static void StyleGrid(DataGridView grid)
    {
        grid.BackgroundColor = Surface;
        grid.BorderStyle = BorderStyle.FixedSingle;
        grid.GridColor = Line;
        grid.RowHeadersVisible = false;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.AllowUserToResizeRows = false;
        grid.ReadOnly = true;
        grid.MultiSelect = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.EnableHeadersVisualStyles = false;
        grid.ColumnHeadersHeight = 38;
        grid.ColumnHeadersDefaultCellStyle.BackColor = Primary;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        grid.ColumnHeadersDefaultCellStyle.Font = BoldFont;
        grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Primary;
        grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;
        grid.DefaultCellStyle.Font = BaseFont;
        grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(224, 242, 238);
        grid.DefaultCellStyle.SelectionForeColor = PrimaryDark;
        grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 251, 253);
        grid.RowTemplate.Height = 32;
        grid.AutoGenerateColumns = false;
    }
}
