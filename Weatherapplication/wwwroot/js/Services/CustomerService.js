app.service("CustomerService", function ($http) {

    this.Save = function (customer) {
        return $http.post("/api/Customerapi/Save", customer);
    };

    this.GetAll = function () {
        return $http.get("/api/Customerapi/GetAll");
    };

    this.GetById = function (id) {
        return $http.get("/api/Customerapi/GetById/" + id);
    };

    this.Update = function (customer) {
        return $http.put("/api/Customerapi/Update", customer);
    };

    this.Delete = function (id) {
        return $http.delete("/api/Customerapi/Delete/" + id);
    };

});