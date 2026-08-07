app.controller("CustomerController", function ($scope, CustomerService) {

    $scope.Customer = {
        isActive: true,
        partytype: 1
    };

    $scope.CustomerList = [];

    // Page Load
    $scope.ShowGrid = true;
    $scope.ShowForm = false;

    LoadCustomers();

    //==========================
    // Load Customer List
    //==========================
    function LoadCustomers() {

        CustomerService.GetAll().then(function (response) {

            $scope.CustomerList = response.data;

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

    //==========================
    // New Customer
    //==========================
    $scope.NewCustomer = function () {

        $scope.Clear();

        $scope.ShowGrid = false;
        $scope.ShowForm = true;

    };

    //==========================
    // Save / Update
    //==========================
    $scope.SaveCustomer = function () {

        if (!$scope.Customer.id) {
            $scope.Customer.partytype = $scope.IsSupplier ? 2 : 1;
            CustomerService.Save($scope.Customer).then(function () {
                
                Swal.fire({
                    icon: 'success',
                    title: 'Success',
                    text: 'Customer Saved Successfully'
                });

                LoadCustomers();

                $scope.Clear();

                $scope.ShowForm = false;
                $scope.ShowGrid = true;

            });

        }
        else {

            CustomerService.Update($scope.Customer).then(function () {

                Swal.fire({
                    icon: 'success',
                    title: 'Success',
                    text: 'Customer Updated Successfully'
                });

                LoadCustomers();

                $scope.Clear();

                $scope.ShowForm = false;
                $scope.ShowGrid = true;

            });

        }

    };

    //==========================
    // Edit
    //==========================
    $scope.Edit = function (id) {

        CustomerService.GetById(id).then(function (response) {

            $scope.Customer = response.data;
           // alert(id);
            $scope.ShowGrid = false;
            $scope.ShowForm = true;

        });

    };

    //==========================
    // Delete
    //==========================
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

                CustomerService.Delete(id).then(function () {

                    Swal.fire(
                        'Deleted!',
                        'Customer Deleted Successfully.',
                        'success'
                    );

                    LoadCustomers();

                });

            }

        });

    };
    $scope.GetPartyType = function (partytype) {

        switch (partytype) {
            case 1:
                return "Customer";
            case 2:
                return "Supplier";
            default:
                return "-";
        }
    };
    //==========================
    // Clear
    //==========================
    $scope.Clear = function () {
        $scope.Customer = {
            partytype: 1
        };
        $scope.Customer = {
            isActive: true
        };

    };

    //==========================
    // Cancel
    //==========================
    $scope.Cancel = function () {

        $scope.Clear();

        $scope.ShowGrid = true;
        $scope.ShowForm = false;

    };

});