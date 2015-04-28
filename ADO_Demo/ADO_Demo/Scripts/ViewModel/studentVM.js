var urlPath = window.location.pathname;

$(function () {
    ko.applyBindings(studentViewModel);
    StudentVM.getStudents();
    StudentVM.firstName();

});

var studentViewModel = function () {

    StudentVM = this;

    StudentVM.Students = ko.observableArray([]);

    StudentVM.getStudents = function () {
        var self = this;
        $.ajax({
            type: "GET",
            url: '/Student/FetchStudents',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.Students(data); //Put the response in ObservableArray

            },
            error: function (err) {
                alert(err.status + " : " + err.statusText);
            }
        });
    };

    //$('.firstname').html(StudentVM.Students()[0]);

    StudentVM.firstName = function (student) {
        $('.firstname').html(StudentVM.Students()[0].Age);
    };


    //StudentVM.editStudent = function (student) {
    //    window.location.href = '/Student/Edit/' + student.StudentId;
    //};
    //StudentVM.deleteStudent = function (student) {
    //    window.location.href = '/Student/Delete/' + student.StudentId;
    //};

}



//Model
function Students(data) {
    StudentVM.StudentId = ko.observable(data.StudentId);
    StudentVM.FirstName = ko.observable(data.FirstName);
    StudentVM.LastName = ko.observable(data.LastName);
    StudentVM.Age = ko.observable(data.Age);
    StudentVM.Gender = ko.observable(data.Gender);
    StudentVM.Batch = ko.observable(data.Batch);
    StudentVM.Address = ko.observable(data.Address);
    StudentVM.Class = ko.observable(data.Class);
    StudentVM.School = ko.observable(data.School);
    StudentVM.Domicile = ko.observable(data.Domicile);
}


