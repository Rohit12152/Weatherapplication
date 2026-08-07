app.controller("PurchaseInvoiceController", function ($scope, PurchaseInvoiceService, $timeout) {

    // ==========================================
    // 1. FILTER INIT & SETUP
    // ==========================================
    $scope.Filter = {};

    $scope.ResetFilter = function () {
        var today = new Date();
        var priorDate = new Date();
        priorDate.setMonth(today.getMonth() - 1); // 1 Month Ago Default

        $scope.Filter = {
            fromDate: priorDate,
            toDate: today,
            customerName: ''
        };

        $scope.ApplyFilter();
    };

    $scope.ApplyFilter = function () {
        // Redraw jQuery DataTable to evaluate custom search logic
        if ($.fn.DataTable.isDataTable('#itemTable')) {
            $('#itemTable').DataTable().draw();
        }
    };

    // DataTables Custom Range and Search Engine
    // DataTables Custom Range and Search Engine
    $.fn.dataTable.ext.search.push(
        function (settings, data, dataIndex) {
            if (settings.nTable.id !== 'itemTable') return true;

            var dateStr = data[2];       // Invoice Date column
            var customerStr = data[3];   // Customer Name column

            // 1. Customer Dropdown Exact Match Filter
            if ($scope.Filter.customerName) {
                var selectedCustomer = $scope.Filter.customerName.toLowerCase().trim();
                var rowCustomer = (customerStr || '').toLowerCase().trim();

                if (rowCustomer !== selectedCustomer) {
                    return false;
                }
            }

            // 2. Date Range Filtering (Format: DD-MM-YYYY)
            if (dateStr && ($scope.Filter.fromDate || $scope.Filter.toDate)) {
                var parts = dateStr.trim().split('-');
                if (parts.length === 3) {
                    var rowDate = new Date(parts[2], parts[1] - 1, parts[0]);
                    rowDate.setHours(0, 0, 0, 0);

                    if ($scope.Filter.fromDate) {
                        var from = new Date($scope.Filter.fromDate);
                        from.setHours(0, 0, 0, 0);
                        if (rowDate < from) return false;
                    }

                    if ($scope.Filter.toDate) {
                        var to = new Date($scope.Filter.toDate);
                        to.setHours(23, 59, 59, 999);
                        if (rowDate > to) return false;
                    }
                }
            }

            return true;
        }
    );

    // ==========================================
    // 2. EXISTING CORE FUNCTIONALITY
    // ==========================================
    $scope.PurchaseInvoice = {
        purchaseInvoiceNo: "",
        reference: "",
        customerId: "",
        purchaseInvoiceDate: new Date(),
        purchaseInvoiceItem: [],
        totalAmount: 0,
        totalTax: 0,
        netAmount: 0
    };

    function LoadCustomers() {
        PurchaseInvoiceService.GetCustomerList().then(function (response) {
            $scope.CustomerList = response.data;
        }, function (error) {
            console.log(error);
        });
    }

    function LoadNextInvoiceNo() {
        PurchaseInvoiceService.GetNextPurchaseInvoiceNo()
            .then(function (response) {
                $scope.PurchaseInvoice.purchaseInvoiceNo = response.data;
            });
    }

    function LoadItems() {
        PurchaseInvoiceService.GetItemist().then(function (response) {
            $scope.ItemList = response.data;
        }, function (error) {
            console.log(error);
        });
    }

    $scope.AddRow = function () {
        $scope.PurchaseInvoice.purchaseInvoiceItem.push({
            itemId: "",
            qty: 1,
            rate: 0,
            amount: 0,
            taxPercent: 18,
            taxAmount: 0,
            totalAmount: 0
        });
    };

    $scope.RemoveRow = function (index) {
        $scope.PurchaseInvoice.purchaseInvoiceItem.splice(index, 1);

        var totalAmount = 0;
        var totalTax = 0;
        var netAmount = 0;

        angular.forEach($scope.PurchaseInvoice.purchaseInvoiceItem, function (x) {
            totalAmount += parseFloat(x.amount) || 0;
            totalTax += parseFloat(x.taxAmount) || 0;
            netAmount += parseFloat(x.totalAmount) || 0;
        });

        $scope.PurchaseInvoice.totalAmount = totalAmount.toFixed(2);
        $scope.PurchaseInvoice.totalTax = totalTax.toFixed(2);
        $scope.PurchaseInvoice.netAmount = netAmount.toFixed(2);
    };

    $scope.Calculate = function (item) {
        item.amount = (parseFloat(item.qty) || 0) * (parseFloat(item.rate) || 0);
        item.taxAmount = item.amount * (parseFloat(item.taxPercent) || 0) / 100;
        item.totalAmount = item.amount + item.taxAmount;

        var totalAmount = 0;
        var totalTax = 0;
        var netAmount = 0;

        angular.forEach($scope.PurchaseInvoice.purchaseInvoiceItem, function (x) {
            totalAmount += parseFloat(x.amount) || 0;
            totalTax += parseFloat(x.taxAmount) || 0;
            netAmount += parseFloat(x.totalAmount) || 0;
        });

        $scope.PurchaseInvoice.totalAmount = totalAmount;
        $scope.PurchaseInvoice.totalTax = totalTax;
        $scope.PurchaseInvoice.netAmount = netAmount;
    };

    $scope.Save = function () {
        if (!$scope.PurchaseInvoice.id) {
            PurchaseInvoiceService.Save($scope.PurchaseInvoice).then(function (response) {
                Swal.fire({
                    icon: 'success',
                    title: 'Success',
                    text: 'Purchase Invoice Saved Successfully'
                });
                LoadPurchaseInvoice();
                $scope.Clear();
                $scope.ShowForm = false;
                $scope.ShowGrid = true;
            });
        }
        else {
            PurchaseInvoiceService.Update($scope.PurchaseInvoice).then(function () {
                Swal.fire({
                    icon: 'success',
                    title: 'Success',
                    text: 'Purchase Invoice Updated Successfully'
                });
                LoadPurchaseInvoice();
                $scope.Clear();
                $scope.ShowForm = false;
                $scope.ShowGrid = true;
            });
        }
    };

    $scope.Edit = function (id) {
        PurchaseInvoiceService.GetById(id).then(function (response) {
            $scope.PurchaseInvoice = response.data;
            if ($scope.PurchaseInvoice.purchaseInvoiceDate) {
                $scope.PurchaseInvoice.purchaseInvoiceDate = new Date($scope.PurchaseInvoice.purchaseInvoiceDate);
            }
            if (!$scope.PurchaseInvoice.purchaseInvoiceItem) {
                $scope.PurchaseInvoice.purchaseInvoiceItem = [];
            }

            $scope.ShowGrid = false;
            $scope.ShowForm = true;
        });
    };

    $scope.ConverttoInvoice = function (id) {
        PurchaseInvoiceService.ConverttoPurchaseInvoice(id)
            .then(function (response) {
                $scope.PurchaseInvoice = response.data;

                if ($scope.PurchaseInvoice.purchaseInvoiceDate) {
                    $scope.PurchaseInvoice.purchaseInvoiceDate = new Date($scope.PurchaseInvoice.purchaseInvoiceDate);
                }

                $scope.ShowGrid = false;
                $scope.ShowForm = true;
            })
            .catch(function (error) {
                console.log(error);
            });
    };

    // Add flag to scope initialization
    $scope.IsGridLoaded = false;

    function LoadPurchaseInvoice() {
        // Hide table during load/re-render
        $scope.IsGridLoaded = false;

        PurchaseInvoiceService.GetAll().then(function (response) {
            $scope.PurchaseInvoiceList = response.data;

            $timeout(function () {
                if ($.fn.DataTable.isDataTable('#itemTable')) {
                    $('#itemTable').DataTable().destroy();
                }

                $('#itemTable').DataTable({
                    destroy: true,
                    responsive: true
                });

                // Apply filters first
                $scope.ApplyFilter();

                // Reveal the grid ONLY after DataTables has completely rendered
                $scope.IsGridLoaded = true;

            }, 200);
        });
    }

    $scope.Delete = function (id) {
        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be able to recover this record!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Yes, Delete',
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.isConfirmed) {
                PurchaseInvoiceService.Delete(id).then(function () {
                    Swal.fire(
                        'Deleted!',
                        'Purchase Invoice Deleted Successfully.',
                        'success'
                    );
                    LoadPurchaseInvoice();
                });
            }
        });
    };

    $scope.Clear = function () {
        $scope.PurchaseInvoice = {
            id: 0,
            purchaseInvoiceNo: "",
            reference: "",
            customerId: null,
            purchaseInvoiceDate: new Date(),
            purchaseInvoiceItem: [],
            totalAmount: 0,
            totalTax: 0,
            netAmount: 0
        };

        $scope.AddRow();
    };

    $scope.NewPI = function () {
        $scope.Clear();
        $scope.ShowGrid = false;
        $scope.ShowForm = true;
    };

    $scope.Cancel = function () {
        $scope.Clear();
        $scope.ShowGrid = true;
        $scope.ShowForm = false;
    };

    // ==========================================
    // 3. INITIAL EXECUTION
    // ==========================================
    if (poid && poid !== "") {
        $scope.ConverttoInvoice(poid);
    }

    $scope.ResetFilter(); // Initializes default 1-month date scope values
    LoadNextInvoiceNo();
    LoadCustomers();
    LoadItems();
    LoadPurchaseInvoice();
    $scope.AddRow();
    $scope.ShowGrid = true;
    $scope.ShowForm = false;
});