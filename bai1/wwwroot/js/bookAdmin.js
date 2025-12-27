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

// Hàm parse giá từ string (hỗ trợ format: 150000, 150.000, 150,000)
function parsePriceValue(priceString) {
    if (!priceString) return 0;
    
    var cleaned = priceString.toString().trim()
        .replace(/₫/g, '')
        .replace(/VND/gi, '')
        .replace(/\s/g, '');
    
    // Xử lý format Việt Nam (150.000)
    if (cleaned.indexOf('.') !== -1 && cleaned.indexOf(',') === -1) {
        cleaned = cleaned.replace(/\./g, '');
    }
    // Nếu có cả dấu chấm và dấu phẩy
    else if (cleaned.indexOf('.') !== -1 && cleaned.indexOf(',') !== -1) {
        cleaned = cleaned.replace(/\./g, '').replace(',', '.');
    }
    // Nếu chỉ có dấu phẩy
    else if (cleaned.indexOf(',') !== -1) {
        var parts = cleaned.split(',');
        if (parts.length === 2 && parts[1].length === 3) {
            cleaned = cleaned.replace(/,/g, '');
        } else {
            cleaned = cleaned.replace(',', '.');
        }
    }
    
    return parseFloat(cleaned) || 0;
}

function nextStep() {
    if (currentStep < 3) {
        const currentDiv = document.getElementById(`step-${currentStep}`);
        console.log("currentDivStep .... ", currentDiv);
        const inputs = currentDiv.querySelectorAll(".required-field");
        
        // Kiểm tra Step 1
        if (currentStep === 1) {
            // Kiểm tra các trường text thông thường
            const isbn = document.getElementById('ISBN');
            const publishDate = document.getElementById('publishDate');
            const title = document.getElementById('productTitle');
            const authorIds = document.getElementById('AuthorIds');
            
            if (!isbn.value.trim()) {
                alert("Vui lòng nhập ISBN!");
                isbn.focus();
                return;
            }
            
            if (!publishDate.value.trim()) {
                alert("Vui lòng chọn ngày xuất bản!");
                publishDate.focus();
                return;
            }
            
            if (!title.value.trim()) {
                alert("Vui lòng nhập tên sách!");
                title.focus();
                return;
            }
            
            // Kiểm tra tác giả - có thể chưa lưu vào hidden input
            if (!authorIds.value.trim() && authorSelected.length === 0) {
                alert("Vui lòng chọn ít nhất một tác giả!");
                document.getElementById('AuthorSearch').focus();
                return;
            }
            
            // Nếu có author đã chọn nhưng chưa lưu vào hidden input
            if (authorSelected.length > 0 && !authorIds.value.trim()) {
                authorIds.value = JSON.stringify(authorSelected);
            }
        }
        
        // Kiểm tra Step 2 - Giá và tồn kho
        if (currentStep === 2) {
            const costPriceInput = document.getElementById('costprice');
            const listPriceInput = document.getElementById('listprice');
            const salePriceInput = document.getElementById('saleprice');
            const inventoryInput = document.getElementById('inventory');
            
            const costPrice = parsePriceValue(costPriceInput.value);
            const listPrice = parsePriceValue(listPriceInput.value);
            const salePrice = parsePriceValue(salePriceInput.value);
            const inventory = parseInt(inventoryInput.value) || 0;
            
            if (costPrice <= 0) {
                alert("Vui lòng nhập giá nhập hợp lệ!");
                costPriceInput.focus();
                return;
            }
            
            if (listPrice <= 0) {
                alert("Vui lòng nhập giá niêm yết hợp lệ!");
                listPriceInput.focus();
                return;
            }
            
            if (salePrice <= 0) {
                alert("Vui lòng nhập giá bán hợp lệ!");
                salePriceInput.focus();
                return;
            }
            
            // Giá bán phải <= giá niêm yết
            if (salePrice > listPrice) {
                alert("Giá bán không được cao hơn giá niêm yết!");
                salePriceInput.focus();
                return;
            }
            
            // Cảnh báo nếu giá bán < giá nhập (lỗ vốn)
            if (salePrice < costPrice) {
                if (!confirm("Cảnh báo: Giá bán thấp hơn giá nhập (lỗ vốn). Bạn có muốn tiếp tục?")) {
                    salePriceInput.focus();
                    return;
                }
            }
            
            if (inventory < 0) {
                alert("Số lượng tồn kho phải >= 0!");
                inventoryInput.focus();
                return;
            }
            
            if (!inventoryInput.value.trim()) {
                alert("Vui lòng nhập số lượng tồn kho!");
                inventoryInput.focus();
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






