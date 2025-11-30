

//Trigger logic keydown 'enter' for form submission
const form = document.querySelector('form');
document.addEventListener('keypress', function (e) {
    if (e.key === 'Enter' && (e.target.tagName === 'INPUT' || e.target.tagName === 'TEXTAREA')) {
        e.preventDefault();
        return false;
    }
});

let debounceTimer;
let authorSelected = [];
document.getElementById('AuthorSearch').addEventListener('keyup', function () {
    const keyword = this.value;

    clearTimeout(debounceTimer);

    if (keyword === '') {
        const suggestionBox = document.getElementById('AuthorSuggestions');
        suggestionBox.style.display = 'none';
        renderSelectedAuthors();
        return;
    }

    debounceTimer = setTimeout(() => {
        searchAuthor(keyword);
    }, 500);

});


async function searchAuthor(keyword) {

    if (keyword.length < 2) {
        return;
    }

    const response = await fetch(`/Author/Search?name=${encodeURIComponent(keyword)}`);
    const authors = await response.json();
    console.log(authors);
    let html = "";

    if (authors.data.authors.length > 0) {
        authors.data.authors.forEach(a => {
            html += `
                <div class="list-group-item list-group-item-action author-item"
                data-id="${a.id}"
                data-name="${a.name}"
                data-description="${a.description}
                ">
                ${a.name} | ${a.description}
                </div>
            `;
        });
    }

    html += `
        <div class="list-group-item list-group-item-action text-primary"
        id="AddNewAuthor">
        + Add new author: "${keyword}"
        </div>
    `;

    const suggestionBox = document.getElementById('AuthorSuggestions');
    suggestionBox.innerHTML = html;
    suggestionBox.style.display = 'block';
}

//document.addEventListener('DOMContentLoaded', () => {
//  document.addEventListener('click', async function (e) {
//      if ( e.target && e.target.id === 'AddNewAuthor') {
//        console.log(e.target, "e target");
//        const name = document.getElementById('AuthorSearch').value;
//          if (!name) return;
//            const response = await fetch('/Author/CreateAuthors', {
//                  method: 'POST',
//                  headers: {
//                       'Content-Type': 'application/x-www-form-urlencoded'
//                  },
//                  body: `title=${encodeURIComponent(name)}`
//            });
//          const author = await response.json();
//          console.log("authors", author);
//          authorSelected.push({
//              authorName: author.data.name,
//          })
//          renderSelectedAuthors();
//      }
//      if (e.target && e.target.classList.contains('author-item')) {
//          const authorId = e.target.dataset.id;
//          const authorName = e.target.dataset.name;
//          const description = e.target.dataset.description.trim();
//          if (!authorSelected.some(a => a.id === authorId)) {
//              authorSelected.push({
//                  authorId, authorName, description
//              })
//          }
//          console.log(authorSelected, "authorSelected");
//          renderSelectedAuthors();
//      }
//  });
//});

function renderSelectedAuthors() {
    const authorIdInput = document.getElementById('AuthorIds');
    const authorSearch = document.getElementById('AuthorSearch');
    let html = "";
    authorSelected.forEach(author => {
        html += `<ul style="display: d-flex gap-3"
                    data-id="${author?.Id}"
                    data-name="${author?.Name}" >
                      <li> ${author?.Name} | ${author?.description || ''}  </li>
                    </ul>`
    });
    const suggestionBox = document.getElementById('AuthorSuggestions');
    suggestionBox.innerHTML = html;
    authorSearch.value = "";
    console.log("authorSelected ", authorSelected);
    authorIdInput.value = authorSelected.length ? JSON.stringify(authorSelected) : '';
    console.log("authorIdInput.value", authorIdInput.value);
    suggestionBox.style.display = authorSelected.length > 0 ? 'block' : 'none';
}

let translatorSelected = [];

document.addEventListener('DOMContentLoaded', () => {

    // --- XỬ LÝ CLICK CHUNG CHO CẢ TRANG ---
    document.addEventListener('click', async function (e) {

        // ---------------------------------------------
        // PHẦN 1: XỬ LÝ AUTHORS (Giữ nguyên logic của bạn + Fix lỗi ID)
        // ---------------------------------------------

        // CASE 1.1: Tạo mới Author
        if (e.target && e.target.id === 'AddNewAuthor') {
            const name = document.getElementById('AuthorSearch').value;
            if (!name) return;

            // Gọi API tạo mới
            const response = await fetch('/Author/CreateAuthors', {
                method: 'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body: `title=${encodeURIComponent(name)}` // Backend bạn nhận 'title' hay 'Name'? Check lại nhé
            });
            const res = await response.json();

            if (res.isSuccess) {
                // QUAN TRỌNG: Phải lấy ID trả về từ server
                authorSelected.push({
                    Name: res.data.name
                });
                renderSelectedAuthors();
            }
        }

        // CASE 1.2: Chọn Author có sẵn từ list gợi ý
        if (e.target && e.target.closest('.author-item')) {
            const item = e.target.closest('.author-item'); // Dùng closest để click vào thẻ con cũng nhận
            const authorId = item.dataset.id;
            const authorName = item.dataset.name;

            if (!authorSelected.some(a => a.Id === parseInt(authorId))) {
                authorSelected.push({ Id: parseInt(authorId), Name: authorName });
            }
            renderSelectedAuthors();
        }


        // ---------------------------------------------
        // PHẦN 2: XỬ LÝ TRANSLATORS (Tương tự Author)
        // ---------------------------------------------
        document.getElementById('TranslatorSearch').addEventListener('keyup', function () {
            const keyword = this.value;

            clearTimeout(debounceTimer);

            if (keyword === '') {
                const suggestionBox = document.getElementById('TranslatorSuggestions');
                suggestionBox.style.display = 'none';
                renderSelectedTranslators();
                return;
            }

            debounceTimer = setTimeout(() => {
                searchTranslator(keyword);
            }, 500);

        });

        async function searchTranslator(keyword) {

            if (keyword.length < 2) {
                return;
            }

            const response = await fetch(`/Translator/Search?name=${encodeURIComponent(keyword)}`);
            const translators = await response.json();
            console.log(translators);
            let html = "";

            if (translators.data.translators.length > 0) {
                translators.data.translators.forEach(a => {
                    html += `
                <div class="list-group-item list-group-item-action translator-item"
                data-id="${a.id}"
                data-name="${a.name}"
                data-description="${a.description}
                ">
                ${a.name} | ${a.description}
                </div>
            `;
                });
            }

            html += `
        <div class="list-group-item list-group-item-action text-primary"
        id="AddNewTranslator">
        + Add new translator: "${keyword}"
        </div>
    `;

            const suggestionBox = document.getElementById('TranslatorSuggestions');
            suggestionBox.innerHTML = html;
            suggestionBox.style.display = 'block';
        }

        // CASE 2.1: Tạo mới Translator
        // (Bạn có thể dùng chung API CreateAuthors nếu bảng DB giống nhau, hoặc tạo API mới)
        if (e.target && e.target.id === 'AddNewTranslator') {
            const name = document.getElementById('TranslatorSearch').value;
            if (!name) return;

            const response = await fetch('/Translator/CreateAuthors', { // Dùng chung API tạo Person
                method: 'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body: `title=${encodeURIComponent(name)}`
            });
            const res = await response.json();

            if (res.isSuccess) {
                translatorSelected.push({
                    Id: res.data.Id, // Lưu ID mới
                    Name: res.data.name
                });
                renderSelectedTranslators(); // Hàm render riêng
            }
        }

        // CASE 2.2: Chọn Translator có sẵn
        if (e.target && e.target.closest('.translator-item')) {
            const item = e.target.closest('.translator-item');
            const translatorId = item.dataset.id;
            const translatorName = item.dataset.name;

            if (!translatorSelected.some(t => t.Id === parseInt(translatorId))) {
                translatorSelected.push({ Id: parseInt(translatorId), Name: translatorName });
            }
            renderSelectedTranslators();
        }
    });


});

// --- HÀM RENDER RIÊNG CHO TRANSLATOR ---
function renderSelectedTranslators() {
    const inputHidden = document.getElementById('TranslatorData');
    const displayBox = document.getElementById('SelectedTranslators'); // Div hiển thị riêng
    const searchBox = document.getElementById('TranslatorSearch');
    const suggestBox = document.getElementById('TranslatorSuggestions');

    let html = "";
    translatorSelected.forEach((item, index) => {
        // Thêm nút X để xóa
        html += `
            <span class="badge bg-info text-dark me-2">
                ${item.Name}
                <i class="fa fa-times" style="cursor:pointer" onclick="removeTranslator(${index})"></i>
            </span>`;
    });

    displayBox.innerHTML = html;

    // Cập nhật input ẩn
    inputHidden.value = translatorSelected.length ? JSON.stringify(translatorSelected) : '';

    // Reset ô tìm kiếm
    searchBox.value = "";
    suggestBox.innerHTML = "";
    suggestBox.style.display = 'none';
}

// Hàm xóa translator khỏi danh sách đã chọn
function removeTranslator(index) {
    translatorSelected.splice(index, 1);
    renderSelectedTranslators();
}

    const fileInput = document.getElementById('book-cover');
    const previewArea = document.getElementById('preview-area');
    const changeText = document.getElementById('text-upload');
    fileInput.addEventListener('change', function(event) {
    const file = event.target.files[0];
    if (file) {
          const imageUrl = URL.createObjectURL(file);
          previewArea.innerHTML = `
            <div class="preview-box">
                <img src="${imageUrl}" alt="Preview" class="preview-image">
                    <p class="file-name">${file.name}</p>
            </div>
              `;
        changeText.innerText = 'Change Image';
    }});

let currentStep = 1;


function showStep(step) {
    document.querySelectorAll(".step").forEach(s => s.classList.remove("active"));
    document.getElementById(`step-${step}`).classList.add("active");

    document.querySelectorAll(".step-item").forEach(s => s.classList.remove("active"));
    document.getElementById(`indicator-${step}`).classList.add("active");

}

function nextStep() {
    if (currentStep < 3) {
        const currentDiv = document.getElementById(`step-${currentStep}`);
        console.log("currentDivStep .... ", currentDiv);
        const inputs = currentDiv.querySelectorAll(".required-field")
        let costPrice = 0;

        for (let input of inputs) {
            console.log("Input ....", input);
            if (input.value.trim() === "") {
                alert("Please fill all fields before proceeding.");
                input.focus();
                return;
            }

            if (input.id === "costprice") {
                costPrice = parseFloat(input.value);
            }

            if (input.id === "saleprice") {
                const salePrice = parseFloat(input.value);
                if (salePrice < costPrice) {
                    alert("Price must be higher orginal price.");
                    input.focus();
                    return;
                }
                else if (salePrice < 0) {
                    alert("Price must be 0 or higher");
                    input.focus();
                    return;
                }
            }

            if (input.id === "inventory" && parseInt(input.value) < 0) {
                alert("Inventory must be 0 or higher.");
                input.focus();
                return;
            }   
        }
        currentStep++;
        console.log("currentStep ....", currentStep); 
        showStep(currentStep);
    }
}

function prevStep() {
    if (currentStep >= 1) {
        currentStep--;
        showStep(currentStep);
    }
}






