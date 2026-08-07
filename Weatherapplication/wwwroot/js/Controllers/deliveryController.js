app.controller("deliveryController", function ($scope, deliveryService, $timeout) {
    $scope.DeliveryOrder = {
        doNo: "",
        reference: "",
        customerId: null,
        doDate: new Date(),
        deliveryOrderItem: [],
        totalAmount: 0,
        totalTax: 0,
        netAmount: 0
    };
    function LoadCustomers() {

        deliveryService.GetCustomerList().then(function (response) {
            //console.log(response.data);
            //console.log(response.data[0]);
            $scope.CustomerList = response.data;
        }, function (error) {
            console.log(error);
        });
    }
    function LoadNextDONo() {
        deliveryService.GetNextDONo()
            .then(function (response) {
                $scope.DeliveryOrder.doNo = response.data;
            });
    }
    function LoadItems() {

        deliveryService.GetItemist().then(function (response) {
            //console.log(response.data);
            //console.log(response.data[0]);
            $scope.ItemList = response.data;
        }, function (error) {
            console.log(error);
        });
    }
    function LoadItemsCategory() {

        deliveryService.GetItemcategory().then(function (response) {
            $scope.ItemListCategory = response.data;
            console.log($scope.ItemListCategory);
        }, function (error) {
            console.log(error);
        });
    }
    $scope.CategoryChanged = function () {

        $scope.Delivery.ItemId = null;

        deliveryService.GetItemListByCategory($scope.Delivery.CategoryId)
            .then(function (response) {

                $scope.ItemList = response.data;

            }, function (error) {
                console.log(error);
            });
    };
    $scope.AddRow = function () {

        $scope.DeliveryOrder.deliveryOrderItem.push({
            itemId: "",
            soQty: 1,
            qty: 1,
            rate: 0,
            amount: 0,
            gst: 0,
            taxAmount: 0,
            totalAmount: 0
        });

    };
    $scope.ValidateDOQty = function (item) {

        var soQtyso = parseFloat(item.soQty) || 0;
        var doQty = parseFloat(item.qty) || 0;

        if (doQty > soQtyso) {
            alert("DO Quantity cannot be greater than SO Quantity.");

            item.qty = soQtyso;   // ya 0 bhi kar sakte hain

            doQty = soQtyso;
        }

        $scope.Calculate(item);
    };
    $scope.RemoveRow = function (index) {

        $scope.DeliveryOrder.deliveryOrderItem.splice(index, 1);

        var totalAmount = 0;
        var totalTax = 0;
        var netAmount = 0;

        angular.forEach($scope.DeliveryOrder.deliveryOrderItem, function (x) {
            totalAmount += parseFloat(x.amount) || 0;
            totalTax += parseFloat(x.taxAmount) || 0;
            netAmount += parseFloat(x.totalAmount) || 0;
        });

        $scope.DeliveryOrder.totalAmount = totalAmount.toFixed(2);
        $scope.DeliveryOrder.totalTax = totalTax.toFixed(2);
        $scope.DeliveryOrder.netAmount = netAmount.toFixed(2);
    };

    $scope.Calculate = function (item) {

        //  alert("Calculate Called");

        item.amount = (parseFloat(item.qty) || 0) * (parseFloat(item.rate) || 0);
        item.taxAmount = item.amount * (parseFloat(item.gst) || 0) / 100;
        item.totalAmount = item.amount + item.taxAmount;

        var totalAmount = 0;
        var totalTax = 0;
        var netAmount = 0;

        angular.forEach($scope.DeliveryOrder.deliveryOrderItem, function (x) {
            totalAmount += parseFloat(x.amount) || 0;
            totalTax += parseFloat(x.taxAmount) || 0;
            netAmount += parseFloat(x.totalAmount) || 0;
        });

        //console.log(totalAmount, totalTax, netAmount);

        $scope.DeliveryOrder.totalAmount = totalAmount;
        $scope.DeliveryOrder.totalTax = totalTax;
        $scope.DeliveryOrder.netAmount = netAmount;
    }
    $scope.Save = function () {
        console.log("Save Button Clicked");
        console.log($scope.DeliveryOrder);
        if (!$scope.DeliveryOrder.id) {

            deliveryService.Save($scope.DeliveryOrder).then(function (response) {
                console.log("Response:", response.data);
                Swal.fire({
                    icon: 'success',
                    title: 'Success',
                    text: 'Delivery Order Saved Successfully'
                }).then(function () {
                    LoadDeliveryOrderList();
                    $scope.Clear();
                    $scope.ShowForm = false;
                    $scope.ShowGrid = true;
                    window.location.href = "/DeliveryOrder/Index";

                });
            })
        }
        else {
            deliveryService.Update($scope.DeliveryOrder).then(function () {

                Swal.fire({
                    icon: 'success',
                    title: 'Success',
                    text: 'Delivery Order Updated Successfully'
                });
                LoadDeliveryOrderList();
                $scope.Clear();

                $scope.ShowForm = false;
                $scope.ShowGrid = true;

            });
        }
    };

    $scope.Edit = function (id) {

        deliveryService.GetById(id).then(function (response) {

            $scope.DeliveryOrder = response.data;
            if ($scope.DeliveryOrder.DODate) {
                $scope.DeliveryOrder.DODate = new Date($scope.DeliveryOrder.DODate);
            }
            if (!$scope.DeliveryOrder.DeliveryOrderItem) {
                $scope.DeliveryOrder.DeliveryOrderItem = [];
            }

            $scope.ShowGrid = false;
            $scope.ShowForm = true;
        });

    };

    $scope.Clear = function () {
        $scope.DeliveryOrder = {
            id: 0,
            doNo: "",
            reference: "",
            customerId: null,
            doDate: new Date(),
            deliveryOrderItem: [],
            totalAmount: 0,
            totalTax: 0,
            netAmount: 0
        };
        $scope.AddRow();
    };
    function LoadDeliveryOrderList() {

        deliveryService.GetAll().then(function (response) {

            $scope.DeliveryOrderList = response.data;

            $timeout(function () {

                if ($.fn.DataTable.isDataTable('#itemTable')) {
                    $('#itemTable').DataTable().destroy();
                }

                $('#itemTable').DataTable({
                    destroy: true,
                    responsive: true
                });

            }, 300);

        });
    }


    //PAGE LOAD

    if (soid && parseInt(soid) > 0) {

        deliveryService.ConverttoDO(parseInt(soid))
            .then(function (response) {

                console.log("Response:", response.data);

                $scope.DeliveryOrder = response.data;

                if ($scope.DeliveryOrder.doDate) {
                    $scope.DeliveryOrder.doDate = new Date($scope.DeliveryOrder.doDate);
                }

            })
            .catch(function (error) {
                console.log(error);
            });
    }
    $scope.Cancel = function () {
        $scope.Clear();
        $scope.ShowGrid = true;
        $scope.ShowForm = false;

    };
    // PAGE LOAD

    $scope.ShowGrid = true;
    $scope.ShowForm = false;

    // URL se soid nikalo
    var soid = new URLSearchParams(window.location.search).get("soid");

    // Common Data Load
    LoadNextDONo();
    LoadCustomers();
    LoadItemsCategory();
    LoadItems();

    // Grid ke liye
    LoadDeliveryOrderList();

    // New Row
    $scope.AddRow();

    // Agar SO se DO banana hai
    if (soid) {

        $scope.ShowGrid = false;
        $scope.ShowForm = true;

        // SO Data Load
        deliveryService.ConverttoDO(soid).then(function (response) {

            $scope.DeliveryOrder = response.data;

        }, function (error) {
            console.log(error);
        });
    }
    //$scope.ShowGrid = true;
    //$scope.ShowForm = true;
   
});