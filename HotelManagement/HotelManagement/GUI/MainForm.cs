using System.Data;
using HotelManagement.BUS;
using HotelManagement.DTO;

namespace HotelManagement.GUI;

public class MainForm : Form
{
    private readonly PhongBUS _phongBus = new();
    private readonly CheckInBUS _checkInBus = new();
    private readonly DichVuBUS _dichVuBus = new();
    private readonly CheckOutBUS _checkOutBus = new();

    private readonly TextBox _txtCccd = new();
    private readonly TextBox _txtHoTen = new();
    private readonly TextBox _txtSoDienThoai = new();
    private readonly ComboBox _cboEmptyRoom = new();
    private readonly Label _lblLookupStatus = new();
    private readonly Button _btnLookup = UiTheme.CreateButton("Tra cứu", UiTheme.Primary, 95, 34);
    private readonly Button _btnCheckIn = UiTheme.CreateButton("Check-in nhận phòng", UiTheme.Success, 190, 40);
    private readonly Button _btnRefreshCheckIn = UiTheme.CreateButton("Làm mới", UiTheme.Muted, 110, 36);
    private readonly DataGridView _dgvActive = new();

    private readonly ComboBox _cboServiceInvoice = new();
    private readonly ComboBox _cboService = new();
    private readonly NumericUpDown _numServiceQty = new();
    private readonly Button _btnAddService = UiTheme.CreateButton("Thêm dịch vụ", UiTheme.Primary, 140, 38);
    private readonly Button _btnRefreshService = UiTheme.CreateButton("Làm mới", UiTheme.Muted, 110, 36);
    private readonly DataGridView _dgvServices = new();
    private readonly Label _lblServiceTotal = new();

    private readonly ComboBox _cboCheckoutInvoice = new();
    private readonly Button _btnPreviewCheckout = UiTheme.CreateButton("Tính tiền", UiTheme.Primary, 110, 36);
    private readonly Label _lblCheckoutRoom = new();
    private readonly Label _lblCheckoutCustomer = new();
    private readonly Label _lblCheckoutTime = new();
    private readonly Label _lblHours = new();
    private readonly Label _lblRoomFee = new();
    private readonly Label _lblServiceFee = new();
    private readonly Label _lblTotal = new();
    private readonly Button _btnConfirmCheckout = UiTheme.CreateButton("Xác nhận thanh toán", UiTheme.Danger, 190, 40);
    private readonly DataGridView _dgvCheckoutServices = new();
    private CheckoutDTO? _checkout;

    private readonly ComboBox _cboFilterType = new();
    private readonly ComboBox _cboFilterStatus = new();
    private readonly Button _btnReloadRooms = UiTheme.CreateButton("Làm mới", UiTheme.Primary, 100, 34);
    private readonly DataGridView _dgvRooms = new();
    private readonly Label _lblRoomCount = new();
    private DataTable? _roomTable;
    private DataView? _roomView;

    private bool _dangNapDuLieu;

    public MainForm()
    {
        Text = "Hotel Management - Quản lý khách sạn";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(1200, 760);
        Size = new Size(1420, 870);
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
        banner.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 380));
        banner.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        banner.Controls.Add(new Label
        {
            Text = "HOTEL MANAGEMENT",
            Dock = DockStyle.Fill,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 19F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 0);

        banner.Controls.Add(new Label
        {
            Text = "Check-in  •  Dịch vụ  •  Check-out  •  Tìm kiếm phòng",
            Dock = DockStyle.Fill,
            ForeColor = Color.FromArgb(227, 232, 250),
            Font = new Font("Segoe UI", 10.5F),
            TextAlign = ContentAlignment.MiddleRight
        }, 1, 0);

        var tabs = new TabControl
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10F),
            Padding = new Point(20, 6)
        };
        tabs.TabPages.Add(BuildCheckInTab());
        tabs.TabPages.Add(BuildServiceTab());
        tabs.TabPages.Add(BuildCheckoutTab());
        tabs.TabPages.Add(BuildRoomSearchTab());

        root.Controls.Add(banner, 0, 0);
        root.Controls.Add(tabs, 0, 1);
        Controls.Add(root);
    }

    private TabPage BuildCheckInTab()
    {
        var tab = new TabPage("  Check-in  ")
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
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 480));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var leftCard = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = UiTheme.Surface,
            Padding = new Padding(16),
            ColumnCount = 1,
            RowCount = 3,
            Margin = new Padding(0, 0, 10, 0)
        };
        leftCard.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        leftCard.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        leftCard.RowStyles.Add(new RowStyle(SizeType.Absolute, 62));
        leftCard.Controls.Add(UiTheme.CreateSectionTitle("CHECK-IN NHẬN PHÒNG"), 0, 0);

        var form = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 5,
            Margin = new Padding(0)
        };
        form.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 108));
        form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        form.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
        form.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
        form.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
        form.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
        form.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        form.Controls.Add(CreateFormLabel("CCCD"), 0, 0);
        var cccdPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = new Padding(0)
        };
        cccdPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        cccdPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 105));
        _txtCccd.Dock = DockStyle.Fill;
        _txtCccd.MaxLength = 20;
        _txtCccd.Margin = new Padding(0, 9, 8, 9);
        _txtCccd.TextChanged += TxtCccd_TextChanged;
        _btnLookup.Dock = DockStyle.Fill;
        _btnLookup.Margin = new Padding(0, 9, 0, 9);
        _btnLookup.Click += BtnLookup_Click;
        cccdPanel.Controls.Add(_txtCccd, 0, 0);
        cccdPanel.Controls.Add(_btnLookup, 1, 0);
        form.Controls.Add(cccdPanel, 1, 0);

        form.Controls.Add(CreateFormLabel("Họ tên"), 0, 1);
        _txtHoTen.Dock = DockStyle.Fill;
        _txtHoTen.MaxLength = 100;
        _txtHoTen.Margin = new Padding(0, 9, 0, 9);
        form.Controls.Add(_txtHoTen, 1, 1);

        form.Controls.Add(CreateFormLabel("Điện thoại"), 0, 2);
        _txtSoDienThoai.Dock = DockStyle.Fill;
        _txtSoDienThoai.MaxLength = 15;
        _txtSoDienThoai.Margin = new Padding(0, 9, 0, 9);
        form.Controls.Add(_txtSoDienThoai, 1, 2);

        form.Controls.Add(CreateFormLabel("Phòng trống"), 0, 3);
        _cboEmptyRoom.DropDownStyle = ComboBoxStyle.DropDownList;
        _cboEmptyRoom.Dock = DockStyle.Fill;
        _cboEmptyRoom.Margin = new Padding(0, 9, 0, 9);
        form.Controls.Add(_cboEmptyRoom, 1, 3);

        _lblLookupStatus.Text = "Nhập CCCD và bấm Tra cứu để kiểm tra khách hàng.";
        _lblLookupStatus.Dock = DockStyle.Fill;
        _lblLookupStatus.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
        _lblLookupStatus.ForeColor = UiTheme.Muted;
        _lblLookupStatus.TextAlign = ContentAlignment.TopLeft;
        _lblLookupStatus.Padding = new Padding(0, 12, 0, 0);
        form.Controls.Add(_lblLookupStatus, 0, 4);
        form.SetColumnSpan(_lblLookupStatus, 2);
        leftCard.Controls.Add(form, 0, 1);

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Padding = new Padding(0, 10, 0, 0),
            Margin = new Padding(0)
        };
        _btnCheckIn.Click += BtnCheckIn_Click;
        _btnRefreshCheckIn.Click += (_, _) => RunRefreshAll();
        actions.Controls.Add(_btnCheckIn);
        actions.Controls.Add(_btnRefreshCheckIn);
        leftCard.Controls.Add(actions, 0, 2);

        var rightCard = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = UiTheme.Surface,
            Padding = new Padding(16),
            ColumnCount = 1,
            RowCount = 2
        };
        rightCard.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        rightCard.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        rightCard.Controls.Add(UiTheme.CreateSectionTitle("PHÒNG ĐANG LƯU TRÚ"), 0, 0);

        UiTheme.StyleGrid(_dgvActive);
        _dgvActive.Dock = DockStyle.Fill;
        _dgvActive.Margin = new Padding(0);
        _dgvActive.Columns.Add(MakeColumn("SoPhong", "Phòng", 18));
        _dgvActive.Columns.Add(MakeColumn("HoTen", "Khách hàng", 38));
        _dgvActive.Columns.Add(MakeColumn("CCCD", "CCCD", 28));
        _dgvActive.Columns.Add(MakeColumn("NgayNhanPhong", "Giờ nhận", 26, "dd/MM/yyyy HH:mm"));
        rightCard.Controls.Add(_dgvActive, 0, 1);

        layout.Controls.Add(leftCard, 0, 0);
        layout.Controls.Add(rightCard, 1, 0);
        tab.Controls.Add(layout);
        return tab;
    }

    private TabPage BuildServiceTab()
    {
        var tab = new TabPage("  Gọi dịch vụ  ")
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
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 440));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var leftCard = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = UiTheme.Surface,
            Padding = new Padding(16),
            ColumnCount = 1,
            RowCount = 3,
            Margin = new Padding(0, 0, 10, 0)
        };
        leftCard.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        leftCard.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        leftCard.RowStyles.Add(new RowStyle(SizeType.Absolute, 62));
        leftCard.Controls.Add(UiTheme.CreateSectionTitle("GỌI DỊCH VỤ"), 0, 0);

        var form = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 4,
            Margin = new Padding(0)
        };
        form.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 96));
        form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        form.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
        form.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
        form.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
        form.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        form.Controls.Add(CreateFormLabel("Phiếu"), 0, 0);
        _cboServiceInvoice.DropDownStyle = ComboBoxStyle.DropDownList;
        _cboServiceInvoice.Dock = DockStyle.Fill;
        _cboServiceInvoice.Margin = new Padding(0, 10, 0, 10);
        _cboServiceInvoice.SelectedIndexChanged += ServiceInvoice_SelectedIndexChanged;
        form.Controls.Add(_cboServiceInvoice, 1, 0);

        form.Controls.Add(CreateFormLabel("Dịch vụ"), 0, 1);
        _cboService.DropDownStyle = ComboBoxStyle.DropDownList;
        _cboService.Dock = DockStyle.Fill;
        _cboService.Margin = new Padding(0, 10, 0, 10);
        _cboService.SelectedIndexChanged += (_, _) => UpdateServiceButtons();
        form.Controls.Add(_cboService, 1, 1);

        form.Controls.Add(CreateFormLabel("Số lượng"), 0, 2);
        var quantityPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Margin = new Padding(0)
        };
        _numServiceQty.Minimum = 1;
        _numServiceQty.Maximum = 100;
        _numServiceQty.Value = 1;
        _numServiceQty.Width = 100;
        _numServiceQty.Margin = new Padding(0, 12, 0, 8);
        quantityPanel.Controls.Add(_numServiceQty);
        form.Controls.Add(quantityPanel, 1, 2);

        var note = new Label
        {
            Text = "Dịch vụ được cộng dồn vào phiếu đang lưu trú.\n" +
                   "Chọn phiếu ở cột bên trái để xem các dịch vụ đã dùng.",
            Dock = DockStyle.Fill,
            ForeColor = UiTheme.Muted,
            Font = new Font("Segoe UI", 9F, FontStyle.Italic),
            Padding = new Padding(0, 16, 0, 0)
        };
        form.Controls.Add(note, 0, 3);
        form.SetColumnSpan(note, 2);
        leftCard.Controls.Add(form, 0, 1);

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Padding = new Padding(0, 10, 0, 0),
            Margin = new Padding(0)
        };
        _btnAddService.Click += BtnAddService_Click;
        _btnRefreshService.Click += (_, _) => RunRefreshAll();
        actions.Controls.Add(_btnAddService);
        actions.Controls.Add(_btnRefreshService);
        leftCard.Controls.Add(actions, 0, 2);

        var rightCard = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = UiTheme.Surface,
            Padding = new Padding(16),
            ColumnCount = 1,
            RowCount = 2
        };
        rightCard.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        rightCard.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var gridHeader = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = new Padding(0)
        };
        gridHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        gridHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220));
        gridHeader.Controls.Add(UiTheme.CreateSectionTitle("DỊCH VỤ ĐÃ DÙNG"), 0, 0);
        _lblServiceTotal.Text = "Tổng: 0 đ";
        _lblServiceTotal.Dock = DockStyle.Fill;
        _lblServiceTotal.TextAlign = ContentAlignment.MiddleRight;
        _lblServiceTotal.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
        _lblServiceTotal.ForeColor = UiTheme.Danger;
        gridHeader.Controls.Add(_lblServiceTotal, 1, 0);
        rightCard.Controls.Add(gridHeader, 0, 0);

        UiTheme.StyleGrid(_dgvServices);
        _dgvServices.Dock = DockStyle.Fill;
        _dgvServices.Margin = new Padding(0);
        _dgvServices.Columns.Add(MakeColumn("TenDV", "Dịch vụ", 38));
        _dgvServices.Columns.Add(MakeColumn("SoLuong", "SL", 12, "N0", true));
        _dgvServices.Columns.Add(MakeColumn("DonGia", "Đơn giá", 18, "N0", true));
        _dgvServices.Columns.Add(MakeColumn("ThanhTien", "Thành tiền", 18, "N0", true));
        _dgvServices.Columns.Add(MakeColumn("ThoiGian", "Thời gian", 22, "dd/MM/yyyy HH:mm"));
        rightCard.Controls.Add(_dgvServices, 0, 1);

        layout.Controls.Add(leftCard, 0, 0);
        layout.Controls.Add(rightCard, 1, 0);
        tab.Controls.Add(layout);
        return tab;
    }

    private TabPage BuildCheckoutTab()
    {
        var tab = new TabPage("  Check-out  ")
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
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 520));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var leftCard = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = UiTheme.Surface,
            Padding = new Padding(16),
            ColumnCount = 1,
            RowCount = 5,
            Margin = new Padding(0, 0, 10, 0)
        };
        leftCard.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        leftCard.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
        leftCard.RowStyles.Add(new RowStyle(SizeType.Absolute, 86));
        leftCard.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        leftCard.RowStyles.Add(new RowStyle(SizeType.Absolute, 62));
        leftCard.Controls.Add(UiTheme.CreateSectionTitle("CHECK-OUT & HÓA ĐƠN"), 0, 0);

        var selector = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = new Padding(0)
        };
        selector.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        selector.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        _cboCheckoutInvoice.DropDownStyle = ComboBoxStyle.DropDownList;
        _cboCheckoutInvoice.Dock = DockStyle.Fill;
        _cboCheckoutInvoice.Margin = new Padding(0, 10, 8, 10);
        _cboCheckoutInvoice.SelectedIndexChanged += CheckoutInvoice_SelectedIndexChanged;
        _btnPreviewCheckout.Dock = DockStyle.Fill;
        _btnPreviewCheckout.Margin = new Padding(0, 10, 0, 10);
        _btnPreviewCheckout.Click += BtnPreviewCheckout_Click;
        selector.Controls.Add(_cboCheckoutInvoice, 0, 0);
        selector.Controls.Add(_btnPreviewCheckout, 1, 0);
        leftCard.Controls.Add(selector, 0, 1);

        var info = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Margin = new Padding(0)
        };
        info.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        info.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
        info.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        _lblCheckoutRoom.Text = "Chưa chọn phiếu";
        _lblCheckoutRoom.Dock = DockStyle.Fill;
        _lblCheckoutRoom.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        _lblCheckoutRoom.ForeColor = UiTheme.Primary;
        _lblCheckoutCustomer.Text = string.Empty;
        _lblCheckoutCustomer.Dock = DockStyle.Fill;
        _lblCheckoutCustomer.ForeColor = UiTheme.Muted;
        _lblCheckoutTime.Text = "Chọn phiếu lưu trú và bấm Tính tiền.";
        _lblCheckoutTime.Dock = DockStyle.Fill;
        _lblCheckoutTime.Font = UiTheme.SmallFont;
        _lblCheckoutTime.ForeColor = UiTheme.Muted;
        info.Controls.Add(_lblCheckoutRoom, 0, 0);
        info.Controls.Add(_lblCheckoutCustomer, 0, 1);
        info.Controls.Add(_lblCheckoutTime, 0, 2);
        leftCard.Controls.Add(info, 0, 2);

        var summaryGroup = new GroupBox
        {
            Text = "Chi tiết thanh toán",
            Dock = DockStyle.Fill,
            Font = UiTheme.BoldFont,
            ForeColor = UiTheme.Primary,
            Padding = new Padding(12),
            Margin = new Padding(0, 4, 0, 0)
        };

        var summary = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 4,
            Margin = new Padding(0)
        };
        summary.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58));
        summary.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42));
        summary.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
        summary.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
        summary.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
        summary.RowStyles.Add(new RowStyle(SizeType.Percent, 25));

        summary.Controls.Add(CreateSummaryCaption("Số giờ lưu trú"), 0, 0);
        ConfigureSummaryValue(_lblHours, "0 giờ", false);
        summary.Controls.Add(_lblHours, 1, 0);

        summary.Controls.Add(CreateSummaryCaption("Tiền phòng"), 0, 1);
        ConfigureSummaryValue(_lblRoomFee, "0 đ", false);
        summary.Controls.Add(_lblRoomFee, 1, 1);

        summary.Controls.Add(CreateSummaryCaption("Tiền dịch vụ"), 0, 2);
        ConfigureSummaryValue(_lblServiceFee, "0 đ", false);
        summary.Controls.Add(_lblServiceFee, 1, 2);

        summary.Controls.Add(CreateSummaryCaption("TỔNG CỘNG"), 0, 3);
        ConfigureSummaryValue(_lblTotal, "0 đ", true);
        summary.Controls.Add(_lblTotal, 1, 3);

        summaryGroup.Controls.Add(summary);
        leftCard.Controls.Add(summaryGroup, 0, 3);

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Padding = new Padding(0, 10, 0, 0),
            Margin = new Padding(0)
        };
        _btnConfirmCheckout.Enabled = false;
        _btnConfirmCheckout.Click += BtnConfirmCheckout_Click;
        actions.Controls.Add(_btnConfirmCheckout);
        leftCard.Controls.Add(actions, 0, 4);

        var rightCard = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = UiTheme.Surface,
            Padding = new Padding(16),
            ColumnCount = 1,
            RowCount = 2
        };
        rightCard.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        rightCard.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        rightCard.Controls.Add(UiTheme.CreateSectionTitle("DỊCH VỤ ĐÃ DÙNG"), 0, 0);

        UiTheme.StyleGrid(_dgvCheckoutServices);
        _dgvCheckoutServices.Dock = DockStyle.Fill;
        _dgvCheckoutServices.Margin = new Padding(0);
        _dgvCheckoutServices.Columns.Add(MakeColumn("TenDV", "Dịch vụ", 38));
        _dgvCheckoutServices.Columns.Add(MakeColumn("SoLuong", "SL", 12, "N0", true));
        _dgvCheckoutServices.Columns.Add(MakeColumn("DonGia", "Đơn giá", 18, "N0", true));
        _dgvCheckoutServices.Columns.Add(MakeColumn("ThanhTien", "Thành tiền", 18, "N0", true));
        _dgvCheckoutServices.Columns.Add(MakeColumn("ThoiGian", "Thời gian", 22, "dd/MM/yyyy HH:mm"));
        rightCard.Controls.Add(_dgvCheckoutServices, 0, 1);

        layout.Controls.Add(leftCard, 0, 0);
        layout.Controls.Add(rightCard, 1, 0);
        tab.Controls.Add(layout);
        return tab;
    }

    private TabPage BuildRoomSearchTab()
    {
        var tab = new TabPage("  Tìm kiếm phòng  ")
        {
            BackColor = UiTheme.Background,
            Padding = new Padding(12),
            UseVisualStyleBackColor = false
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = UiTheme.Background
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 74));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var filterCard = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = UiTheme.Surface,
            Padding = new Padding(16),
            Margin = new Padding(0, 0, 0, 10)
        };

        var filters = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoScroll = true
        };
        filters.Controls.Add(new Label
        {
            Text = "Loại phòng",
            AutoSize = true,
            Font = UiTheme.BaseFont,
            Margin = new Padding(0, 9, 8, 0)
        });
        _cboFilterType.DropDownStyle = ComboBoxStyle.DropDownList;
        _cboFilterType.Width = 170;
        _cboFilterType.Margin = new Padding(0, 5, 22, 0);
        _cboFilterType.SelectedIndexChanged += (_, _) => ApplyRoomFilter();
        filters.Controls.Add(_cboFilterType);

        filters.Controls.Add(new Label
        {
            Text = "Trạng thái",
            AutoSize = true,
            Font = UiTheme.BaseFont,
            Margin = new Padding(0, 9, 8, 0)
        });
        _cboFilterStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        _cboFilterStatus.Width = 180;
        _cboFilterStatus.Margin = new Padding(0, 5, 22, 0);
        _cboFilterStatus.SelectedIndexChanged += (_, _) => ApplyRoomFilter();
        filters.Controls.Add(_cboFilterStatus);

        _btnReloadRooms.Margin = new Padding(0, 5, 20, 0);
        _btnReloadRooms.Click += (_, _) => RunRefreshAll();
        filters.Controls.Add(_btnReloadRooms);

        _lblRoomCount.Text = "0 phòng";
        _lblRoomCount.AutoSize = true;
        _lblRoomCount.Font = UiTheme.BoldFont;
        _lblRoomCount.ForeColor = UiTheme.Primary;
        _lblRoomCount.Margin = new Padding(0, 13, 0, 0);
        filters.Controls.Add(_lblRoomCount);

        filterCard.Controls.Add(filters);
        layout.Controls.Add(filterCard, 0, 0);

        var gridCard = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = UiTheme.Surface,
            Padding = new Padding(16),
            ColumnCount = 1,
            RowCount = 2
        };
        gridCard.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        gridCard.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        gridCard.Controls.Add(UiTheme.CreateSectionTitle("DANH SÁCH PHÒNG (DataView / RowFilter)"), 0, 0);

        UiTheme.StyleGrid(_dgvRooms);
        _dgvRooms.Dock = DockStyle.Fill;
        _dgvRooms.Margin = new Padding(0);
        _dgvRooms.Columns.Add(MakeColumn("SoPhong", "Số phòng", 20));
        _dgvRooms.Columns.Add(MakeColumn("LoaiPhong", "Loại phòng", 28));
        _dgvRooms.Columns.Add(MakeColumn("GiaTheoGio", "Giá theo giờ", 25, "N0", true));
        _dgvRooms.Columns.Add(MakeColumn("TrangThai", "Trạng thái", 25));
        _dgvRooms.CellFormatting += DgvRooms_CellFormatting;
        gridCard.Controls.Add(_dgvRooms, 0, 1);
        layout.Controls.Add(gridCard, 0, 1);

        tab.Controls.Add(layout);
        return tab;
    }

    private static Label CreateFormLabel(string text)
    {
        return new Label
        {
            Text = text,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = UiTheme.BaseFont,
            ForeColor = UiTheme.PrimaryDark
        };
    }

    private static Label CreateSummaryCaption(string text)
    {
        return new Label
        {
            Text = text,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = UiTheme.Muted,
            Font = UiTheme.BaseFont
        };
    }

    private static void ConfigureSummaryValue(Label label, string text, bool highlight)
    {
        label.Text = text;
        label.Dock = DockStyle.Fill;
        label.TextAlign = ContentAlignment.MiddleRight;
        label.Font = highlight
            ? new Font("Segoe UI", 13F, FontStyle.Bold)
            : UiTheme.BoldFont;
        label.ForeColor = highlight ? UiTheme.Danger : UiTheme.PrimaryDark;
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
        RunRefreshAll();
    }

    private void RunRefreshAll()
    {
        try
        {
            RefreshAllData();
        }
        catch (Exception ex)
        {
            ShowDatabaseError(ex);
        }
    }

    private void RefreshAllData()
    {
        _dangNapDuLieu = true;
        try
        {
            LoadActiveInvoices();
            LoadEmptyRooms();
            LoadServiceInvoices();
            LoadServiceCatalog();
            LoadCheckoutInvoices();
            LoadRoomSearch();
        }
        finally
        {
            _dangNapDuLieu = false;
        }

        LoadServicesForSelectedInvoice();
        ResetCheckoutPreview();
        ApplyRoomFilter();
    }

    private void LoadActiveInvoices()
    {
        var table = _checkInBus.GetActiveInvoices();
        _dgvActive.DataSource = null;
        _dgvActive.DataSource = table;
    }

    private void LoadEmptyRooms()
    {
        var rooms = _phongBus.GetEmptyRooms();
        _cboEmptyRoom.DataSource = null;
        _cboEmptyRoom.DataSource = rooms;
        _cboEmptyRoom.DisplayMember = nameof(PhongDTO.MoTa);
        _cboEmptyRoom.ValueMember = nameof(PhongDTO.SoPhong);
    }

    private void LoadServiceInvoices()
    {
        var table = _checkInBus.GetActiveInvoices();
        _cboServiceInvoice.DataSource = null;
        _cboServiceInvoice.DataSource = table;
        _cboServiceInvoice.DisplayMember = "ThongTin";
        _cboServiceInvoice.ValueMember = "MaPhieu";
    }

    private void LoadServiceCatalog()
    {
        var selected = _cboService.SelectedValue;
        var services = _dichVuBus.GetAll();
        _cboService.DataSource = null;
        _cboService.DataSource = services;
        _cboService.DisplayMember = nameof(DichVuDTO.HienThi);
        _cboService.ValueMember = nameof(DichVuDTO.MaDV);

        if (selected is not null && services.Any(service => service.MaDV == Convert.ToInt32(selected)))
        {
            _cboService.SelectedValue = Convert.ToInt32(selected);
        }
        else if (services.Count > 0)
        {
            _cboService.SelectedIndex = 0;
        }
    }

    private void LoadCheckoutInvoices()
    {
        var table = _checkInBus.GetActiveInvoices();
        _cboCheckoutInvoice.DataSource = null;
        _cboCheckoutInvoice.DataSource = table;
        _cboCheckoutInvoice.DisplayMember = "ThongTin";
        _cboCheckoutInvoice.ValueMember = "MaPhieu";
    }

    private void LoadRoomSearch()
    {
        var selectedType = _cboFilterType.SelectedItem?.ToString() ?? "Tất cả";
        var selectedStatus = _cboFilterStatus.SelectedItem?.ToString() ?? "Tất cả";

        _roomTable = _phongBus.GetAllData();
        _roomView = _roomTable.DefaultView;

        var types = _roomTable.DefaultView
            .ToTable(true, "LoaiPhong")
            .Rows
            .Cast<DataRow>()
            .Select(row => row["LoaiPhong"].ToString() ?? string.Empty)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .OrderBy(value => value)
            .ToList();

        _cboFilterType.Items.Clear();
        _cboFilterType.Items.Add("Tất cả");
        foreach (var type in types)
        {
            _cboFilterType.Items.Add(type);
        }

        _cboFilterStatus.Items.Clear();
        _cboFilterStatus.Items.Add("Tất cả");
        _cboFilterStatus.Items.Add("Trống");
        _cboFilterStatus.Items.Add("Đang ở");
        _cboFilterStatus.Items.Add("Đang dọn dẹp");

        _cboFilterType.SelectedItem = types.Contains(selectedType) ? selectedType : "Tất cả";
        _cboFilterStatus.SelectedItem = _cboFilterStatus.Items.Contains(selectedStatus) ? selectedStatus : "Tất cả";

        _dgvRooms.DataSource = null;
        _dgvRooms.DataSource = _roomView;
    }

    private void ApplyRoomFilter()
    {
        if (_roomView is null || _dangNapDuLieu)
        {
            return;
        }

        var conditions = new List<string>();

        if (_cboFilterType.SelectedItem?.ToString() is { } type && type != "Tất cả")
        {
            conditions.Add($"LoaiPhong = '{EscapeFilterValue(type)}'");
        }

        if (_cboFilterStatus.SelectedItem?.ToString() is { } status && status != "Tất cả")
        {
            conditions.Add($"TrangThai = '{EscapeFilterValue(status)}'");
        }

        _roomView.RowFilter = string.Join(" AND ", conditions);
        _lblRoomCount.Text = $"{_roomView.Count} phòng";
    }

    private static string EscapeFilterValue(string value) => value.Replace("'", "''");

    private void LoadServicesForSelectedInvoice()
    {
        if (_cboServiceInvoice.SelectedValue is not int maPhieu)
        {
            _dgvServices.DataSource = null;
            _lblServiceTotal.Text = "Tổng: 0 đ";
            UpdateServiceButtons();
            return;
        }

        var table = _dichVuBus.GetUsedServices(maPhieu);
        _dgvServices.DataSource = null;
        _dgvServices.DataSource = table;

        decimal total = 0;
        foreach (DataRow row in table.Rows)
        {
            total += Convert.ToDecimal(row["ThanhTien"]);
        }

        _lblServiceTotal.Text = $"Tổng: {total:N0} đ";
        UpdateServiceButtons();
    }

    private void UpdateServiceButtons()
    {
        _btnAddService.Enabled =
            _cboServiceInvoice.SelectedValue is int &&
            _cboService.SelectedItem is DichVuDTO;
    }

    private void ServiceInvoice_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_dangNapDuLieu)
        {
            return;
        }

        try
        {
            LoadServicesForSelectedInvoice();
        }
        catch (Exception ex)
        {
            ShowError(ex, "Không thể tải dịch vụ của phiếu.");
        }
    }

    private void BtnLookup_Click(object? sender, EventArgs e)
    {
        try
        {
            var khachHang = _checkInBus.Lookup(_txtCccd.Text);
            if (khachHang is null)
            {
                _txtHoTen.Clear();
                _txtSoDienThoai.Clear();
                _lblLookupStatus.Text = "Khách hàng mới - vui lòng nhập họ tên và số điện thoại.";
                _lblLookupStatus.ForeColor = UiTheme.Orange;
            }
            else
            {
                _txtHoTen.Text = khachHang.HoTen;
                _txtSoDienThoai.Text = khachHang.SoDienThoai;
                _lblLookupStatus.Text = $"Khách hàng đã tồn tại - Mã KH: {khachHang.MaKH}.";
                _lblLookupStatus.ForeColor = UiTheme.Success;
            }
        }
        catch (Exception ex)
        {
            ShowWarning(ex.Message);
        }
    }

    private void TxtCccd_TextChanged(object? sender, EventArgs e)
    {
        if (_dangNapDuLieu)
        {
            return;
        }

        _lblLookupStatus.Text = "Nhấn Tra cứu để kiểm tra khách hàng.";
        _lblLookupStatus.ForeColor = UiTheme.Muted;
    }

    private void BtnCheckIn_Click(object? sender, EventArgs e)
    {
        try
        {
            var soPhong = _cboEmptyRoom.SelectedValue?.ToString() ?? string.Empty;
            var maPhieu = _checkInBus.CheckIn(
                soPhong,
                _txtHoTen.Text,
                _txtCccd.Text,
                _txtSoDienThoai.Text);

            MessageBox.Show(
                this,
                $"Check-in thành công.\nMã phiếu: {maPhieu}\nPhòng: {soPhong}",
                "Thành công",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            ClearCheckInForm();
            RefreshAllData();
        }
        catch (Exception ex)
        {
            ShowError(ex, "Check-in thất bại.");
        }
    }

    private void ClearCheckInForm()
    {
        _dangNapDuLieu = true;
        try
        {
            _txtCccd.Clear();
            _txtHoTen.Clear();
            _txtSoDienThoai.Clear();
            _lblLookupStatus.Text = "Nhập CCCD và bấm Tra cứu để kiểm tra khách hàng.";
            _lblLookupStatus.ForeColor = UiTheme.Muted;
        }
        finally
        {
            _dangNapDuLieu = false;
        }
    }

    private void BtnAddService_Click(object? sender, EventArgs e)
    {
        if (_cboServiceInvoice.SelectedValue is not int maPhieu)
        {
            ShowWarning("Vui lòng chọn phiếu lưu trú.");
            return;
        }

        if (_cboService.SelectedItem is not DichVuDTO dichVu)
        {
            ShowWarning("Vui lòng chọn dịch vụ.");
            return;
        }

        try
        {
            _dichVuBus.AddService(maPhieu, dichVu.MaDV, (int)_numServiceQty.Value);
            LoadServicesForSelectedInvoice();
            _numServiceQty.Value = 1;
            MessageBox.Show(this, "Đã thêm dịch vụ vào phiếu.", "Thành công",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            ShowError(ex, "Không thể thêm dịch vụ.");
        }
    }

    private void CheckoutInvoice_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (!_dangNapDuLieu)
        {
            ResetCheckoutPreview();
        }
    }

    private void ResetCheckoutPreview()
    {
        _checkout = null;
        _lblCheckoutRoom.Text = "Chưa chọn phiếu";
        _lblCheckoutCustomer.Text = string.Empty;
        _lblCheckoutTime.Text = "Chọn phiếu lưu trú và bấm Tính tiền.";
        _lblHours.Text = "0 giờ";
        _lblRoomFee.Text = "0 đ";
        _lblServiceFee.Text = "0 đ";
        _lblTotal.Text = "0 đ";
        _dgvCheckoutServices.DataSource = null;
        _btnConfirmCheckout.Enabled = false;
    }

    private void BtnPreviewCheckout_Click(object? sender, EventArgs e)
    {
        if (_cboCheckoutInvoice.SelectedValue is not int maPhieu)
        {
            ShowWarning("Vui lòng chọn phiếu lưu trú cần thanh toán.");
            return;
        }

        try
        {
            _checkout = _checkOutBus.Preview(maPhieu);
            _lblCheckoutRoom.Text = $"Phòng {_checkout.SoPhong}  •  {_checkout.LoaiPhong}";
            _lblCheckoutCustomer.Text = $"{_checkout.HoTen}  •  CCCD: {_checkout.CCCD}";
            _lblCheckoutTime.Text =
                $"Nhận: {_checkout.NgayNhanPhong:dd/MM/yyyy HH:mm}  •  " +
                $"Trả: {_checkout.NgayTraPhong:dd/MM/yyyy HH:mm}";
            _lblHours.Text = $"{_checkout.SoGio:N0} giờ";
            _lblRoomFee.Text = $"{_checkout.TienPhong:N0} đ";
            _lblServiceFee.Text = $"{_checkout.TienDichVu:N0} đ";
            _lblTotal.Text = $"{_checkout.TongTien:N0} đ";
            _dgvCheckoutServices.DataSource = null;
            _dgvCheckoutServices.DataSource = _checkout.DichVu;
            _btnConfirmCheckout.Enabled = true;
        }
        catch (Exception ex)
        {
            ResetCheckoutPreview();
            ShowError(ex, "Không thể tính tiền.");
        }
    }

    private void BtnConfirmCheckout_Click(object? sender, EventArgs e)
    {
        if (_checkout is null)
        {
            ShowWarning("Vui lòng bấm Tính tiền trước khi thanh toán.");
            return;
        }

        var confirm = MessageBox.Show(
            this,
            $"Xác nhận thanh toán {_checkout.TongTien:N0} đ cho phòng {_checkout.SoPhong}?",
            "Xác nhận thanh toán",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes)
        {
            return;
        }

        try
        {
            _checkOutBus.Confirm(_checkout);
            MessageBox.Show(this, "Check-out thành công. Phòng chuyển sang trạng thái Đang dọn dẹp.",
                "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
            RefreshAllData();
        }
        catch (Exception ex)
        {
            ShowError(ex, "Check-out thất bại.");
        }
    }

    private void DgvRooms_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 ||
            _dgvRooms.Rows[e.RowIndex].DataBoundItem is not DataRowView row)
        {
            return;
        }

        switch (row["TrangThai"].ToString())
        {
            case "Trống":
                e.CellStyle.BackColor = Color.FromArgb(232, 245, 233);
                e.CellStyle.ForeColor = Color.FromArgb(27, 94, 32);
                break;
            case "Đang ở":
                e.CellStyle.BackColor = Color.FromArgb(255, 235, 238);
                e.CellStyle.ForeColor = Color.FromArgb(183, 28, 28);
                break;
            case "Đang dọn dẹp":
                e.CellStyle.BackColor = Color.FromArgb(255, 248, 225);
                e.CellStyle.ForeColor = Color.FromArgb(230, 81, 0);
                break;
        }
    }

    private void ShowWarning(string message)
    {
        MessageBox.Show(this, message, "Thông báo",
            MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    private void ShowError(Exception exception, string message)
    {
        MessageBox.Show(this, $"{message}\n\n{exception.Message}", "Lỗi",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private void ShowDatabaseError(Exception exception)
    {
        MessageBox.Show(
            this,
            "Không thể kết nối cơ sở dữ liệu HotelDB.\n" +
            "Hãy chạy Database/ScriptDB.sql và kiểm tra chuỗi kết nối trong App.config.\n\n" +
            exception.Message,
            "Lỗi cơ sở dữ liệu",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }
}
