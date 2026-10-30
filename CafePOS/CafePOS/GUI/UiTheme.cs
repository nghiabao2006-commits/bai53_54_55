namespace CafePOS.GUI;

internal static class UiTheme
{
    public static readonly Color Background = Color.FromArgb(246, 241, 235);
    public static readonly Color Surface = Color.White;
    public static readonly Color Primary = Color.FromArgb(111, 78, 55);
    public static readonly Color PrimaryDark = Color.FromArgb(78, 52, 35);
    public static readonly Color Success = Color.FromArgb(46, 125, 50);
    public static readonly Color Danger = Color.FromArgb(198, 40, 40);
    public static readonly Color Muted = Color.FromArgb(117, 117, 117);
    public static readonly Color Line = Color.FromArgb(224, 224, 224);
    public static readonly Color Selection = Color.FromArgb(255, 193, 7);

    public static readonly Font BaseFont = new("Segoe UI", 10F);
    public static readonly Font SmallFont = new("Segoe UI", 9F);
    public static readonly Font BoldFont = new("Segoe UI", 10F, FontStyle.Bold);
    public static readonly Font TitleFont = new("Segoe UI", 14F, FontStyle.Bold);
    public static readonly Font BigTitleFont = new("Segoe UI", 16F, FontStyle.Bold);

    public static Button CreateButton(string text, Color background, int width = 118, int height = 38)
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
            Margin = new Padding(5),
            UseVisualStyleBackColor = false
        };
        button.FlatAppearance.BorderSize = 0;
        return button;
    }

    public static Label CreateSectionTitle(string text)
    {
        return new Label
        {
            Text = text,
            Font = TitleFont,
            ForeColor = Primary,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            AutoEllipsis = true
        };
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
        grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(239, 227, 216);
        grid.DefaultCellStyle.SelectionForeColor = PrimaryDark;
        grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 248, 246);
        grid.RowTemplate.Height = 32;
        grid.AutoGenerateColumns = false;
    }
}
