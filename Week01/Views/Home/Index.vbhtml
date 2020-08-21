@Code
    ViewData("Title") = "Home Page"
End Code

<div class="jumbotron">
    <div class="container">
        <h1>Hello! Welcome to week 2! </h1>
        <p>Bootstrap is the most popular HTML, CSS, and JS framework for developing responsive, mobile first projects on the web.</p>
        <p>This page has been created using Bootstrap 3!</p>
    </div>
    <div class="container">
        <div class="row">
            <div class="col-lg-12">
                <p>Click this button to go to the page that was built using pure CSS ! </p>
                <p>&darr;</p>
                <p>
                    <a class="btn btn-primary btn-lg" role="button" href="@Url.Action("About", "Home")">Pure CSS Page</a>
                </p>
            </div>
        </div>
    </div>
</div>