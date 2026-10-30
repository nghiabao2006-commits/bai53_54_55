using System.Data;
using System.Data.SqlClient;
using CafePOS.BUS;
using CafePOS.DTO;

namespace CafePOS.GUI;

public class MainForm : Form
{
    private readonly BanAnBUS _banAnBus = new();
    private readonly MenuBUS _menuBus = new();
    private readonly OrderBUS _orderBus = new();

    private readonly FlowLayoutPanel _flpTables = new();
    private readonly Label _lblSelectedTable = new();
    private readonly Label _lblInvoiceInfo = new();
    private readonly Label _lblOrderTotal = new();
    private readonly DataGridView _dgvOrder = new();
    private readonly ComboBox _cboCategory = new();
    private readonly ComboBox _cboDish = new();
    private readonly NumericUpDown _numQuantity = new();
    private readonly Button _btnAddDish = UiTheme.CreateButton("Thêm món", UiTheme.Primary, 110, 34);
    private readonly Button _btnRemoveDish = UiTheme.CreateButton("Xóa món", Color.FromArgb(239, 108, 0), 110, 36);
    private readonly Button _btnPay = UiTheme.CreateButton("Thanh toán", UiTheme.Danger, 140, 36);
    private readonly Button _btnTransfer = UiTheme.CreateButton("Chuyển bàn", UiTheme.Primary, 120, 36);
    private readonly Button _btnRefresh = UiTheme.CreateButton("Làm mới", UiTheme.Muted, 110, 36);

    private readonly DataGridView _dgvCategories = new();
    private readonly TextBox _txtCategoryName = new();
    private readonly Button _btnCategoryAdd = UiTheme.CreateButton("Thêm", UiTheme.Success, 86, 34);
    private readonly Button _btnCategoryUpdate = UiTheme.CreateButton("Sửa", UiTheme.Primary, 86, 34);
    private readonly Button _btnCategoryDelete = UiTheme.CreateButton("Xóa", UiTheme.Danger, 86, 34);
    private readonly DataGridView _dgvDishes = new();
    private readonly TextBox _txtDishName = new();
    private readonly NumericUpDown _numPrice = new();
    private readonly ComboBox _cboDishCategory = new();
    private readonly Button _btnDishNew = UiTheme.CreateButton("Tạo mới", UiTheme.Muted, 100, 36);
    private readonly Button _btnDishSave = UiTheme.CreateButton("Lưu món", UiTheme.Primary, 110, 36);
    private readonly Button _btnDishDelete = UiTheme.CreateButton("Xóa", UiTheme.Danger, 90, 36);

    private int? _maBanDangChon;
    private HoaDonDTO? _hoaDon;
    private DataTable? _chiTiet;
    private bool _dangSuaMon;
    private bool _dangNapDuLieu;

    public MainForm()
    {
        Text = "CafePOS - Hệ thống quản lý quán cà phê";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(1180, 720);
        Size = new Size(1380, 840);
        BackColor = UiTheme.Background;
        Font = UiTheme.BaseFont;
        AutoScaleMode = AutoScaleMode.Font;
        BuildUi();
        Load += MainForm_Load;
    }

    private void BuildUi()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = UiTheme.Background
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 66));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var banner = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = UiTheme.Primary,
            Padding = new Padding(22, 0, 22, 0)
        };
        banner.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300));
        banner.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        var title = new Label
        {
            Text = "CAFE POS",
            Dock = DockStyle.Fill,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 19F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        };
        var subtitle = new Label
        {
            Text = "Sơ đồ bàn  •  Gọi món  •  Thanh toán  •  Quản lý thực đơn",
            Dock = DockStyle.Fill,
            ForeColor = Color.FromArgb(244, 235, 226),
            Font = new Font("Segoe UI", 10.5F),
            TextAlign = ContentAlignment.MiddleRight
        };

        banner.Controls.Add(title, 0, 0);
        banner.Controls.Add(subtitle, 1, 0);

        var tabs = new TabControl
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10F),
            Padding = new Point(20, 6)
        };
        tabs.TabPages.Add(BuildPosTab());
        tabs.TabPages.Add(BuildMenuTab());

        root.Controls.Add(banner, 0, 0);
        root.Controls.Add(tabs, 0, 1);
        Controls.Add(root);
    }

    private TabPage BuildPosTab()
    {
        var tab = new TabPage("  Sơ đồ bàn & Order  ")
        {
            BackColor = UiTheme.Background,
            Padding = new Padding(12),
            UseVisualStyleBackColor = false
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = UiTheme.Background
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 370));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var leftCard = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = UiTheme.Surface,
            Padding = new Padding(14),
            ColumnCount = 1,
            RowCount = 3,
            Margin = new Padding(0, 0, 10, 0)
        };
        leftCard.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        leftCard.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        leftCard.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        leftCard.Controls.Add(UiTheme.CreateSectionTitle("SƠ ĐỒ BÀN"), 0, 0);

        _flpTables.Dock = DockStyle.Fill;
        _flpTables.BackColor = UiTheme.Surface;
        _flpTables.AutoScroll = true;
        _flpTables.Padding = new Padding(0, 4, 0, 4);
        leftCard.Controls.Add(_flpTables, 0, 1);

        var legend = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Margin = new Padding(0)
        };
        legend.Controls.Add(new Label
        {
            Text = "● Trống",
            ForeColor = UiTheme.Success,
            AutoSize = true,
            Font = UiTheme.SmallFont,
            Margin = new Padding(0, 9, 18, 0)
        });
        legend.Controls.Add(new Label
        {
            Text = "● Có khách",
            ForeColor = UiTheme.Danger,
            AutoSize = true,
            Font = UiTheme.SmallFont,
            Margin = new Padding(0, 9, 0, 0)
        });
        leftCard.Controls.Add(legend, 0, 2);

        var rightCard = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = UiTheme.Surface,
            Padding = new Padding(16),
            ColumnCount = 1,
            RowCount = 4
        };
        rightCard.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
        rightCard.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        rightCard.RowStyles.Add(new RowStyle(SizeType.Absolute, 96));
        rightCard.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));

        var header = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = new Padding(0)
        };
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260));

        var headerText = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Margin = new Padding(0)
        };
        headerText.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        headerText.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        _lblSelectedTable.Text = "Chưa chọn bàn";
        _lblSelectedTable.Dock = DockStyle.Fill;
        _lblSelectedTable.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
        _lblSelectedTable.ForeColor = UiTheme.Primary;
        _lblSelectedTable.TextAlign = ContentAlignment.MiddleLeft;
        _lblInvoiceInfo.Text = "Chọn một bàn để bắt đầu order.";
        _lblInvoiceInfo.Dock = DockStyle.Fill;
        _lblInvoiceInfo.Font = UiTheme.SmallFont;
        _lblInvoiceInfo.ForeColor = UiTheme.Muted;
        _lblInvoiceInfo.TextAlign = ContentAlignment.TopLeft;
        headerText.Controls.Add(_lblSelectedTable, 0, 0);
        headerText.Controls.Add(_lblInvoiceInfo, 0, 1);

        _lblOrderTotal.Text = "Tổng: 0 đ";
        _lblOrderTotal.Dock = DockStyle.Fill;
        _lblOrderTotal.Font = new Font("Segoe UI", 17F, FontStyle.Bold);
        _lblOrderTotal.ForeColor = UiTheme.Danger;
        _lblOrderTotal.TextAlign = ContentAlignment.MiddleRight;

        header.Controls.Add(headerText, 0, 0);
        header.Controls.Add(_lblOrderTotal, 1, 0);
        rightCard.Controls.Add(header, 0, 0);

        UiTheme.StyleGrid(_dgvOrder);
        _dgvOrder.Dock = DockStyle.Fill;
        _dgvOrder.Margin = new Padding(0);
        _dgvOrder.Columns.Add(MakeColumn("TenMon", "Tên món", 55));
        _dgvOrder.Columns.Add(MakeColumn("SoLuong", "SL", 12, "N0", true));
        _dgvOrder.Columns.Add(MakeColumn("DonGia", "Đơn giá", 20, "N0", true));
        _dgvOrder.Columns.Add(MakeColumn("ThanhTien", "Thành tiền", 25, "N0", true));
        _dgvOrder.SelectionChanged += (_, _) => UpdateOrderButtons();
        rightCard.Controls.Add(_dgvOrder, 0, 1);

        var addGroup = new GroupBox
        {
            Text = "Thêm món",
            Dock = DockStyle.Fill,
            Font = UiTheme.BoldFont,
            ForeColor = UiTheme.Primary,
            Padding = new Padding(10),
            Margin = new Padding(0, 8, 0, 0)
        };

        var addFlow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoScroll = true,
            Padding = new Padding(0, 3, 0, 0)
        };

        addFlow.Controls.Add(CreateFieldLabel("Danh mục"));
        _cboCategory.DropDownStyle = ComboBoxStyle.DropDownList;
        _cboCategory.Width = 170;
        _cboCategory.Margin = new Padding(0, 5, 12, 0);
        _cboCategory.SelectedIndexChanged += Category_SelectedIndexChanged;
        addFlow.Controls.Add(_cboCategory);

        addFlow.Controls.Add(CreateFieldLabel("Món"));
        _cboDish.DropDownStyle = ComboBoxStyle.DropDownList;
        _cboDish.Width = 260;
        _cboDish.Margin = new Padding(0, 5, 12, 0);
        _cboDish.SelectedIndexChanged += (_, _) => UpdateOrderButtons();
        addFlow.Controls.Add(_cboDish);

        addFlow.Controls.Add(CreateFieldLabel("SL"));
        _numQuantity.Minimum = 1;
        _numQuantity.Maximum = 999;
        _numQuantity.Value = 1;
        _numQuantity.Width = 62;
        _numQuantity.TextAlign = HorizontalAlignment.Center;
        _numQuantity.Margin = new Padding(0, 5, 10, 0);
        addFlow.Controls.Add(_numQuantity);

        _btnAddDish.Margin = new Padding(0, 5, 0, 0);
        _btnAddDish.Click += BtnAddDish_Click;
        addFlow.Controls.Add(_btnAddDish);
        addGroup.Controls.Add(addFlow);
        rightCard.Controls.Add(addGroup, 0, 2);

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Padding = new Padding(0, 10, 0, 0),
            Margin = new Padding(0)
        };
        _btnPay.Click += BtnPay_Click;
        _btnTransfer.Click += BtnTransfer_Click;
        _btnRemoveDish.Click += BtnRemoveDish_Click;
        _btnRefresh.Click += BtnRefresh_Click;
        actions.Controls.Add(_btnPay);
        actions.Controls.Add(_btnTransfer);
        actions.Controls.Add(_btnRemoveDish);
        actions.Controls.Add(_btnRefresh);
        rightCard.Controls.Add(actions, 0, 3);

        layout.Controls.Add(leftCard, 0, 0);
        layout.Controls.Add(rightCard, 1, 0);
        tab.Controls.Add(layout);
        return tab;
    }

    private TabPage BuildMenuTab()
    {
        var tab = new TabPage("  Quản lý thực đơn  ")
        {
            BackColor = UiTheme.Background,
            Padding = new Padding(12),
            UseVisualStyleBackColor = false
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = UiTheme.Background
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 340));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var categoryCard = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = UiTheme.Surface,
            Padding = new Padding(14),
            ColumnCount = 1,
            RowCount = 3,
            Margin = new Padding(0, 0, 10, 0)
        };
        categoryCard.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        categoryCard.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        categoryCard.RowStyles.Add(new RowStyle(SizeType.Absolute, 116));
        categoryCard.Controls.Add(UiTheme.CreateSectionTitle("DANH MỤC"), 0, 0);

        UiTheme.StyleGrid(_dgvCategories);
        _dgvCategories.Dock = DockStyle.Fill;
        _dgvCategories.Margin = new Padding(0);
        _dgvCategories.Columns.Add(MakeColumn("TenDanhMuc", "Tên danh mục", 100));
        _dgvCategories.SelectionChanged += (_, _) => FillCategoryEditorFromGrid();
        _dgvCategories.CellClick += (_, _) => FillCategoryEditorFromGrid();
        categoryCard.Controls.Add(_dgvCategories, 0, 1);

        var categoryEditor = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Margin = new Padding(0, 8, 0, 0)
        };
        categoryEditor.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        categoryEditor.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        _txtCategoryName.Dock = DockStyle.Fill;
        _txtCategoryName.PlaceholderText = "Nhập tên danh mục...";
        _txtCategoryName.Margin = new Padding(0, 0, 0, 6);
        categoryEditor.Controls.Add(_txtCategoryName, 0, 0);

        var categoryButtons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Margin = new Padding(0)
        };
        _btnCategoryAdd.Click += BtnCategoryAdd_Click;
        _btnCategoryUpdate.Click += BtnCategoryUpdate_Click;
        _btnCategoryDelete.Click += BtnCategoryDelete_Click;
        categoryButtons.Controls.Add(_btnCategoryAdd);
        categoryButtons.Controls.Add(_btnCategoryUpdate);
        categoryButtons.Controls.Add(_btnCategoryDelete);
        categoryEditor.Controls.Add(categoryButtons, 0, 1);

        categoryCard.Controls.Add(categoryEditor, 0, 2);

        var dishCard = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = UiTheme.Surface,
            Padding = new Padding(14),
            ColumnCount = 1,
            RowCount = 3
        };
        dishCard.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        dishCard.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        dishCard.RowStyles.Add(new RowStyle(SizeType.Absolute, 174));
        dishCard.Controls.Add(UiTheme.CreateSectionTitle("DANH SÁCH MÓN"), 0, 0);

        UiTheme.StyleGrid(_dgvDishes);
        _dgvDishes.Dock = DockStyle.Fill;
        _dgvDishes.Margin = new Padding(0);
        _dgvDishes.Columns.Add(MakeColumn("TenMon", "Tên món", 55));
        _dgvDishes.Columns.Add(MakeColumn("TenDanhMuc", "Danh mục", 30));
        _dgvDishes.Columns.Add(MakeColumn("DonGia", "Đơn giá (đ)", 20, "N0", true));
        _dgvDishes.SelectionChanged += (_, _) => FillDishEditorFromGrid();
        _dgvDishes.CellClick += (_, _) => FillDishEditorFromGrid();
        dishCard.Controls.Add(_dgvDishes, 0, 1);

        var dishEditorGroup = new GroupBox
        {
            Text = "Thông tin món",
            Dock = DockStyle.Fill,
            Font = UiTheme.BoldFont,
            ForeColor = UiTheme.Primary,
            Padding = new Padding(12),
            Margin = new Padding(0, 8, 0, 0)
        };

        var dishEditor = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 3,
            Margin = new Padding(0)
        };
        dishEditor.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 78));
        dishEditor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        dishEditor.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 78));
        dishEditor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        dishEditor.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        dishEditor.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        dishEditor.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        dishEditor.Controls.Add(CreateFieldLabel("Tên món"), 0, 0);
        _txtDishName.Dock = DockStyle.Fill;
        _txtDishName.Margin = new Padding(0, 2, 0, 4);
        dishEditor.Controls.Add(_txtDishName, 1, 0);
        dishEditor.SetColumnSpan(_txtDishName, 3);

        dishEditor.Controls.Add(CreateFieldLabel("Danh mục"), 0, 1);
        _cboDishCategory.DropDownStyle = ComboBoxStyle.DropDownList;
        _cboDishCategory.Dock = DockStyle.Fill;
        _cboDishCategory.Margin = new Padding(0, 2, 10, 4);
        dishEditor.Controls.Add(_cboDishCategory, 1, 1);

        dishEditor.Controls.Add(CreateFieldLabel("Đơn giá"), 2, 1);
        _numPrice.Minimum = 0;
        _numPrice.Maximum = 1_000_000_000;
        _numPrice.DecimalPlaces = 0;
        _numPrice.ThousandsSeparator = true;
        _numPrice.Dock = DockStyle.Fill;
        _numPrice.TextAlign = HorizontalAlignment.Right;
        _numPrice.Margin = new Padding(0, 2, 0, 4);
        dishEditor.Controls.Add(_numPrice, 3, 1);

        var dishButtons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Margin = new Padding(0, 4, 0, 0)
        };
        _btnDishNew.Click += BtnDishNew_Click;
        _btnDishSave.Click += BtnDishSave_Click;
        _btnDishDelete.Click += BtnDishDelete_Click;
        dishButtons.Controls.Add(_btnDishSave);
        dishButtons.Controls.Add(_btnDishDelete);
        dishButtons.Controls.Add(_btnDishNew);
        dishEditor.Controls.Add(dishButtons, 0, 2);
        dishEditor.SetColumnSpan(dishButtons, 4);

        dishEditorGroup.Controls.Add(dishEditor);
        dishCard.Controls.Add(dishEditorGroup, 0, 2);

        layout.Controls.Add(categoryCard, 0, 0);
        layout.Controls.Add(dishCard, 1, 0);
        tab.Controls.Add(layout);
        return tab;
    }

    private static Label CreateFieldLabel(string text)
    {
        return new Label
        {
            Text = text,
            AutoSize = true,
            Font = UiTheme.BaseFont,
            Margin = new Padding(0, 9, 6, 0)
        };
    }

    private static DataGridViewTextBoxColumn MakeColumn(
        string propertyName, string headerText, float fillWeight,
        string? format = null, bool alignRight = false)
    {
        var column = new DataGridViewTextBoxColumn
        {
            DataPropertyName = propertyName,
            HeaderText = headerText,
            FillWeight = fillWeight,
            SortMode = DataGridViewColumnSortMode.NotSortable
        };

        if (format is not null)
        {
            column.DefaultCellStyle.Format = format;
        }

        if (alignRight)
        {
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        return column;
    }

    private void MainForm_Load(object? sender, EventArgs e)
    {
        try
        {
            LoadTables();
            LoadMenuTab();
            LoadPosMenu();
            UpdateOrderButtons();
        }
        catch (Exception ex)
        {
            ShowDatabaseError(ex);
        }
    }

    private void LoadTables(int? selectMaBan = null)
    {
        if (selectMaBan.HasValue)
        {
            _maBanDangChon = selectMaBan.Value;
        }

        var tables = _banAnBus.GetAll();
        _flpTables.SuspendLayout();
        _flpTables.Controls.Clear();

        foreach (var ban in tables)
        {
            var button = new Button
            {
                Text = $"{ban.TenBan}\n{ban.TrangThai}",
                Width = 100,
                Height = 76,
                Margin = new Padding(7),
                Tag = ban.MaBan,
                BackColor = ban.CoKhach ? UiTheme.Danger : UiTheme.Success,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false,
                TextAlign = ContentAlignment.MiddleCenter
            };
            button.FlatAppearance.BorderSize = ban.MaBan == _maBanDangChon ? 4 : 1;
            button.FlatAppearance.BorderColor = ban.MaBan == _maBanDangChon
                ? UiTheme.Selection
                : Color.White;
            button.Click += TableButton_Click;
            _flpTables.Controls.Add(button);
        }

        _flpTables.ResumeLayout();
    }

    private void TableButton_Click(object? sender, EventArgs e)
    {
        if (sender is Button { Tag: int maBan })
        {
            SelectTable(maBan);
        }
    }

    private void SelectTable(int maBan)
    {
        try
        {
            var session = _orderBus.OpenTable(maBan);
            var ban = _banAnBus.GetById(maBan);

            _maBanDangChon = maBan;
            _hoaDon = session.HoaDon;
            _chiTiet = session.ChiTiet;

            _lblSelectedTable.Text = ban?.TenBan ?? $"Bàn {maBan}";
            _dgvOrder.DataSource = null;
            _dgvOrder.DataSource = _chiTiet;

            UpdateInvoiceInfo();
            UpdateTotalLabel();
            UpdateOrderButtons();
            LoadTables(maBan);
            _numQuantity.Value = 1;
        }
        catch (Exception ex)
        {
            ShowError(ex, "Không thể mở bàn.");
        }
    }

    private void UpdateInvoiceInfo()
    {
        if (_maBanDangChon is null)
        {
            _lblInvoiceInfo.Text = "Chọn một bàn để bắt đầu order.";
            return;
        }

        _lblInvoiceInfo.Text = _hoaDon is null
            ? "Bàn đang trống • Chưa gọi món"
            : $"Hóa đơn #{_hoaDon.MaHoaDon} • Vào lúc {_hoaDon.GioVao:HH:mm dd/MM/yyyy}";
    }

    private void UpdateTotalLabel()
    {
        var total = _chiTiet is null ? 0 : _orderBus.GetTotal(_chiTiet);
        _lblOrderTotal.Text = $"Tổng: {total:N0} đ";
    }

    private void UpdateOrderButtons()
    {
        var hasTable = _maBanDangChon.HasValue;
        var hasDish = _cboDish.SelectedItem is DataRowView;
        var hasRows = _chiTiet is { Rows.Count: > 0 };

        _btnAddDish.Enabled = hasTable && hasDish;
        _btnRemoveDish.Enabled = hasTable && hasRows && _dgvOrder.CurrentRow is not null;
        _btnPay.Enabled = _hoaDon is not null && hasRows;
        _btnTransfer.Enabled = _hoaDon is not null;
    }

    private void Category_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_dangNapDuLieu)
        {
            return;
        }

        try
        {
            if (_cboCategory.SelectedValue is not null)
            {
                LoadDishesForPos(Convert.ToInt32(_cboCategory.SelectedValue));
            }
        }
        catch (Exception ex)
        {
            ShowError(ex, "Không thể tải thực đơn.");
        }
    }

    private void LoadPosMenu()
    {
        var previousCategory = _cboCategory.SelectedValue;
        var categories = _menuBus.GetCategories();

        _dangNapDuLieu = true;
        try
        {
            _cboCategory.DataSource = null;
            _cboCategory.DataSource = categories;
            _cboCategory.DisplayMember = "TenDanhMuc";
            _cboCategory.ValueMember = "MaDanhMuc";

            if (categories.Rows.Count == 0)
            {
                _cboDish.DataSource = null;
                return;
            }

            if (previousCategory is not null &&
                categories.Rows.Cast<DataRow>().Any(r => Convert.ToInt32(r["MaDanhMuc"]) == Convert.ToInt32(previousCategory)))
            {
                _cboCategory.SelectedValue = Convert.ToInt32(previousCategory);
            }
            else
            {
                _cboCategory.SelectedIndex = 0;
            }

            LoadDishesForPos(Convert.ToInt32(_cboCategory.SelectedValue));
        }
        finally
        {
            _dangNapDuLieu = false;
            UpdateOrderButtons();
        }
    }

    private void LoadDishesForPos(int maDanhMuc)
    {
        var dishes = _menuBus.GetDishes(maDanhMuc);
        _cboDish.DataSource = null;
        _cboDish.DataSource = dishes;
        _cboDish.DisplayMember = "TenMon";
        _cboDish.ValueMember = "MaMon";
        UpdateOrderButtons();
    }

    private void BtnAddDish_Click(object? sender, EventArgs e)
    {
        if (_maBanDangChon is not int maBan || _chiTiet is null)
        {
            MessageBox.Show(this, "Vui lòng chọn bàn trước khi gọi món.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (_cboDish.SelectedItem is not DataRowView selectedDish)
        {
            MessageBox.Show(this, "Vui lòng chọn món ăn.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        try
        {
            _hoaDon ??= _orderBus.EnsureInvoice(maBan);

            var mon = new MonAnDTO
            {
                MaMon = Convert.ToInt32(selectedDish["MaMon"]),
                TenMon = selectedDish["TenMon"].ToString() ?? string.Empty,
                MaDanhMuc = Convert.ToInt32(selectedDish["MaDanhMuc"]),
                DonGia = Convert.ToDecimal(selectedDish["DonGia"])
            };

            _orderBus.AddDish(_chiTiet, _hoaDon, mon, (int)_numQuantity.Value);
            UpdateInvoiceInfo();
            UpdateTotalLabel();
            UpdateOrderButtons();
            LoadTables(maBan);
            _numQuantity.Value = 1;
        }
        catch (Exception ex)
        {
            UpdateInvoiceInfo();
            try
            {
                LoadTables(_maBanDangChon);
            }
            catch
            {
                // Giữ nguyên giao diện hiện tại nếu không tải lại được sơ đồ bàn.
            }

            ShowError(ex, "Không thể thêm món.");
        }
    }

    private void BtnRemoveDish_Click(object? sender, EventArgs e)
    {
        if (_chiTiet is null || _dgvOrder.CurrentRow?.DataBoundItem is not DataRowView rowView)
        {
            MessageBox.Show(this, "Vui lòng chọn món cần xóa.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        try
        {
            _orderBus.RemoveDish(_chiTiet, rowView.Row);
            UpdateTotalLabel();
            UpdateOrderButtons();
        }
        catch (Exception ex)
        {
            ShowError(ex, "Không thể xóa món.");
        }
    }

    private void BtnPay_Click(object? sender, EventArgs e)
    {
        if (_hoaDon is null || _chiTiet is null)
        {
            MessageBox.Show(this, "Bàn chưa có hóa đơn để thanh toán.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (_chiTiet.Rows.Count == 0)
        {
            MessageBox.Show(this, "Bàn chưa gọi món nên không thể thanh toán.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var total = _orderBus.GetTotal(_chiTiet);
        var tableName = _lblSelectedTable.Text;
        var confirm = MessageBox.Show(
            this,
            $"Xác nhận thanh toán {total:N0} đ cho {tableName}?",
            "Thanh toán",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes)
        {
            return;
        }

        try
        {
            _orderBus.Pay(_hoaDon, _chiTiet);
            ClearOrder();
            LoadTables();
            MessageBox.Show(this, "Thanh toán thành công. Bàn đã chuyển về trạng thái Trống.",
                "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            ShowError(ex, "Thanh toán thất bại.");
        }
    }

    private void BtnTransfer_Click(object? sender, EventArgs e)
    {
        if (_hoaDon is null || _maBanDangChon is not int maBanHienTai)
        {
            MessageBox.Show(this, "Bàn chưa có hóa đơn để chuyển.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        try
        {
            var emptyTables = _banAnBus.GetEmptyTables(maBanHienTai);
            if (emptyTables.Count == 0)
            {
                MessageBox.Show(this, "Không còn bàn trống để chuyển đến.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var form = new ChonBanForm(emptyTables);
            if (form.ShowDialog(this) != DialogResult.OK || form.SelectedMaBan is not int maBanDich)
            {
                return;
            }

            _orderBus.TransferTable(_hoaDon, maBanDich);
            MessageBox.Show(this, $"Đã chuyển hóa đơn sang bàn {maBanDich}.", "Thành công",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            SelectTable(maBanDich);
        }
        catch (Exception ex)
        {
            ShowError(ex, "Không thể chuyển bàn.");
        }
    }

    private void BtnRefresh_Click(object? sender, EventArgs e)
    {
        try
        {
            if (_maBanDangChon is int maBan)
            {
                SelectTable(maBan);
            }
            else
            {
                LoadTables();
            }
        }
        catch (Exception ex)
        {
            ShowDatabaseError(ex);
        }
    }

    private void ClearOrder()
    {
        _maBanDangChon = null;
        _hoaDon = null;
        _chiTiet = null;
        _dgvOrder.DataSource = null;
        _lblSelectedTable.Text = "Chưa chọn bàn";
        _lblInvoiceInfo.Text = "Chọn một bàn để bắt đầu order.";
        _lblOrderTotal.Text = "Tổng: 0 đ";
        UpdateOrderButtons();
    }

    private void LoadMenuTab()
    {
        _dangNapDuLieu = true;
        try
        {
            var categories = _menuBus.GetCategories();
            var categoryCombo = categories.Copy();

            _dgvCategories.DataSource = null;
            _dgvCategories.DataSource = categories;

            _cboDishCategory.DataSource = null;
            _cboDishCategory.DataSource = categoryCombo;
            _cboDishCategory.DisplayMember = "TenDanhMuc";
            _cboDishCategory.ValueMember = "MaDanhMuc";

            var dishes = _menuBus.GetDishes();
            _dgvDishes.DataSource = null;
            _dgvDishes.DataSource = dishes;
        }
        finally
        {
            _dangNapDuLieu = false;
        }

        if (_dgvCategories.Rows.Count > 0)
        {
            _dgvCategories.CurrentCell = _dgvCategories.Rows[0].Cells[0];
            _dgvCategories.Rows[0].Selected = true;
            FillCategoryEditorFromGrid();
        }
        else
        {
            _txtCategoryName.Clear();
        }

        if (_dgvDishes.Rows.Count > 0)
        {
            _dgvDishes.CurrentCell = _dgvDishes.Rows[0].Cells[0];
            _dgvDishes.Rows[0].Selected = true;
            FillDishEditorFromGrid();
        }
        else
        {
            ClearDishEditor();
        }
    }

    private void FillCategoryEditorFromGrid()
    {
        if (_dangNapDuLieu || _dgvCategories.CurrentRow?.DataBoundItem is not DataRowView row)
        {
            return;
        }

        _txtCategoryName.Text = row["TenDanhMuc"].ToString();
    }

    private int? GetSelectedCategoryId()
    {
        if (_dgvCategories.CurrentRow?.DataBoundItem is DataRowView row)
        {
            return Convert.ToInt32(row["MaDanhMuc"]);
        }

        return null;
    }

    private void BtnCategoryAdd_Click(object? sender, EventArgs e)
    {
        try
        {
            _menuBus.AddCategory(_txtCategoryName.Text);
            LoadMenuTab();
            LoadPosMenu();
            MessageBox.Show(this, "Đã thêm danh mục.", "Thành công",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (SqlException ex) when (ex.Number is 2601 or 2627)
        {
            MessageBox.Show(this, "Tên danh mục đã tồn tại.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            ShowError(ex, "Không thể thêm danh mục.");
        }
    }

    private void BtnCategoryUpdate_Click(object? sender, EventArgs e)
    {
        var maDanhMuc = GetSelectedCategoryId();
        if (maDanhMuc is null)
        {
            MessageBox.Show(this, "Vui lòng chọn danh mục cần sửa.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        try
        {
            _menuBus.UpdateCategory(maDanhMuc.Value, _txtCategoryName.Text);
            LoadMenuTab();
            LoadPosMenu();
            MessageBox.Show(this, "Đã cập nhật danh mục.", "Thành công",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (SqlException ex) when (ex.Number is 2601 or 2627)
        {
            MessageBox.Show(this, "Tên danh mục đã tồn tại.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            ShowError(ex, "Không thể cập nhật danh mục.");
        }
    }

    private void BtnCategoryDelete_Click(object? sender, EventArgs e)
    {
        var maDanhMuc = GetSelectedCategoryId();
        if (maDanhMuc is null)
        {
            MessageBox.Show(this, "Vui lòng chọn danh mục cần xóa.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (MessageBox.Show(this, "Bạn có chắc muốn xóa danh mục này?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
        {
            return;
        }

        try
        {
            _menuBus.DeleteCategory(maDanhMuc.Value);
            LoadMenuTab();
            LoadPosMenu();
        }
        catch (SqlException ex) when (ex.Number == 547)
        {
            MessageBox.Show(this, "Danh mục đang chứa món ăn nên không thể xóa.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            ShowError(ex, "Không thể xóa danh mục.");
        }
    }

    private void FillDishEditorFromGrid()
    {
        if (_dangNapDuLieu || _dgvDishes.CurrentRow?.DataBoundItem is not DataRowView row)
        {
            return;
        }

        _dangSuaMon = true;
        _txtDishName.Text = row["TenMon"].ToString();
        _numPrice.Value = Math.Clamp(
            Convert.ToDecimal(row["DonGia"]),
            _numPrice.Minimum,
            _numPrice.Maximum);

        var maDanhMuc = Convert.ToInt32(row["MaDanhMuc"]);
        if (_cboDishCategory.DataSource is DataTable categoryTable &&
            categoryTable.Rows.Cast<DataRow>().Any(r => Convert.ToInt32(r["MaDanhMuc"]) == maDanhMuc))
        {
            _cboDishCategory.SelectedValue = maDanhMuc;
        }

        _btnDishSave.Text = "Cập nhật";
    }

    private int? GetSelectedDishId()
    {
        if (_dgvDishes.CurrentRow?.DataBoundItem is DataRowView row)
        {
            return Convert.ToInt32(row["MaMon"]);
        }

        return null;
    }

    private void ClearDishEditor()
    {
        _dangSuaMon = false;
        _txtDishName.Clear();
        _numPrice.Value = 0;
        _btnDishSave.Text = "Lưu món";

        var categoryId = GetSelectedCategoryId();
        if (categoryId.HasValue &&
            _cboDishCategory.DataSource is DataTable categoryTable &&
            categoryTable.Rows.Cast<DataRow>().Any(r => Convert.ToInt32(r["MaDanhMuc"]) == categoryId.Value))
        {
            _cboDishCategory.SelectedValue = categoryId.Value;
        }
        else if (_cboDishCategory.Items.Count > 0)
        {
            _cboDishCategory.SelectedIndex = 0;
        }
    }

    private void BtnDishNew_Click(object? sender, EventArgs e)
    {
        ClearDishEditor();
        _txtDishName.Focus();
    }

    private void BtnDishSave_Click(object? sender, EventArgs e)
    {
        if (_cboDishCategory.SelectedValue is null)
        {
            MessageBox.Show(this, "Vui lòng chọn danh mục cho món ăn.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var maMon = _dangSuaMon ? GetSelectedDishId() : null;
        if (_dangSuaMon && maMon is null)
        {
            MessageBox.Show(this, "Vui lòng chọn món cần cập nhật.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        try
        {
            var mon = new MonAnDTO
            {
                MaMon = maMon ?? 0,
                TenMon = _txtDishName.Text,
                MaDanhMuc = Convert.ToInt32(_cboDishCategory.SelectedValue),
                DonGia = _numPrice.Value
            };

            if (_dangSuaMon)
            {
                _menuBus.UpdateDish(mon);
            }
            else
            {
                _menuBus.AddDish(mon);
            }

            LoadMenuTab();
            LoadPosMenu();
            ClearDishEditor();
            MessageBox.Show(this, "Đã lưu món ăn.", "Thành công",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            ShowError(ex, "Không thể lưu món ăn.");
        }
    }

    private void BtnDishDelete_Click(object? sender, EventArgs e)
    {
        var maMon = GetSelectedDishId();
        if (maMon is null)
        {
            MessageBox.Show(this, "Vui lòng chọn món cần xóa.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (MessageBox.Show(this, "Bạn có chắc muốn xóa món này?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
        {
            return;
        }

        try
        {
            _menuBus.DeleteDish(maMon.Value);
            LoadMenuTab();
            LoadPosMenu();
            ClearDishEditor();
        }
        catch (SqlException ex) when (ex.Number == 547)
        {
            MessageBox.Show(this, "Món ăn đã phát sinh trong hóa đơn nên không thể xóa.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            ShowError(ex, "Không thể xóa món ăn.");
        }
    }

    private void ShowDatabaseError(Exception exception)
    {
        MessageBox.Show(
            this,
            "Không thể kết nối cơ sở dữ liệu CafePOSDB.\n" +
            "Hãy chạy Database/ScriptDB.sql và kiểm tra chuỗi kết nối trong App.config.\n\n" +
            exception.Message,
            "Lỗi cơ sở dữ liệu",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }

    private void ShowError(Exception exception, string message)
    {
        MessageBox.Show(this, $"{message}\n\n{exception.Message}", "Lỗi",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
