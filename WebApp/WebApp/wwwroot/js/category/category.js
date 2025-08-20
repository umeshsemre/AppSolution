// Load Categories
function loadCategories() {
    apiRequest("/api/Category", "GET", null, function (response) {
        let tbody = $("#categoryTable tbody");
        tbody.empty();
        $.each(response.data, function (i, cat) {

            console.log(response);
            tbody.append(`
                <tr>
                   <td>${cat.id}</td>
                    <td>${cat.categoryName}</td>
                    <td>${cat.categoryTypeId}</td>
                    <td>${cat.isActive ? "Yes" : "No"}</td>
                    <td><img src="data:image/png;base64,${cat.base64Image}" width="60"/></td>
                    <td>
                        <button class="btn btn-sm btn-warning" onclick="editCategory(${cat.categoryId})">Edit</button>
                        <button class="btn btn-sm btn-danger" onclick="deleteCategory(${cat.categoryId})">Delete</button>
                    </td>
                </tr>
            `);
        });
    }, function (err) {
        alert("Error loading categories");
    });
}

// Add/Update Category
function saveCategory() {
    let id = $("#categoryId").val();
    let file = $("#imageFile")[0].files[0];

    if (file) {
        let reader = new FileReader();
        reader.onload = function (e) {
            let base64Image = e.target.result.split(",")[1]; // remove prefix
            submitCategory(base64Image);
        };
        reader.readAsDataURL(file);
    } else {
        submitCategory(null);
    }
}

function submitCategory(base64Image) {
    let category = {
        categoryId: $("#categoryId").val() || 0,
        categoryName: $("#categoryName").val(),
        categoryTypeId: $("#categoryTypeId").val(),
        isDeleted: false,
        isActive: $("#isActive").is(":checked"),
        base64Image: base64Image
    };

    let endpoint = category.categoryId == 0 ? "/api/Category/add" : "/api/Category/update"; // adjust if API differs
    let method = "POST"; // as per swagger, add is POST

    apiRequest(endpoint, method, category, function () {
        alert("Category saved");
        loadCategories();
        $("#categoryForm")[0].reset();
        $("#categoryId").val("");
    }, function () {
        alert("Error saving category");
    });
}

// Delete Category
function deleteCategory(id) {
    if (!confirm("Delete this category?")) return;
    apiRequest("/api/Category/" + id, "DELETE", null, function () {
        alert("Category deleted");
        loadCategories();
    }, function () {
        alert("Error deleting category");
    });
}

// Edit Category
function editCategory(id) {
    apiRequest("/api/Category/" + id, "GET", null, function (cat) {
        $("#categoryId").val(cat.categoryId);
        $("#categoryName").val(cat.categoryName);
        $("#categoryTypeId").val(cat.categoryTypeId);
        $("#isActive").prop("checked", cat.isActive);
    });
}

// Init
$(document).ready(function () {
    loadCategories();
});