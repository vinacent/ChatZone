const box = document.createElement('div');
box.setAttribute('id', 'VinaCent_CommentBox');
box.innerHTML = `<a href="https://vinacent.com">Khung bình luận</a> đang được tải...`;
document.querySelector('#VinaCent').before(box);

const host = document.querySelector('#VinaCent').getAttribute('src').split('clients/runtime.js')[0];
const owner = document.querySelector('#VinaCent').getAttribute('data-owner');

const currentLocation = encodeURIComponent(window.location.href).replace("+", "%2B");

function getCurrentQuery() {
    return '?location=' + currentLocation + '&owner=' + encodeURIComponent(owner).replace("+", "%2B");
}

function loadComments() {
    // const container = document.querySelector('#comment-list');
    fetch(host + 'comments/load-comments' + getCurrentQuery())
        .then(response => response.text())
        .then(data => {
            document.querySelector('#comment-list').innerHTML = data;
            // config for reply button
            const replyButtons = document.querySelectorAll('.vc-reply-comment');
            replyButtons.forEach((el) => {
                el.addEventListener('click', function (event) {
                    event.preventDefault();
                    const replyTo = '@' + event.target.getAttribute("data-reply-to") + ', ';
                    document.querySelector('#vinacent-comment-parent').value = event.target.getAttribute("data-reply-id");
                    document.querySelector('#vinacent-comment-area').value = replyTo + document.querySelector('#vinacent-comment-area').value;
                    document.querySelector('#vinacent-comment-area').scrollIntoView();
                })
            });
            // config for like button
            const likeButtons = document.querySelectorAll('.vc-like-comment');
            const likedIdRawJson = localStorage.getItem(btoa(getCurrentQuery())) ?? "[]";
            let liked = [...JSON.parse(likedIdRawJson)];
            likeButtons.forEach((el) => {
                const commentId = el.getAttribute('data-like-id');
                const countElRef = el.getAttribute('data-ref');
                const crrEl = document.querySelector(countElRef);

                if (liked.indexOf(commentId) >= 0) {
                    el.innerHTML = "Đã thích";
                    el.classList.add('vc-comment-liked');
                } else {
                    el.addEventListener('click', function (event) {
                        event.preventDefault()

                        fetch(host + 'comments/like?commentId=' + commentId)
                            .then(response => response.text())
                            .then(data => {
                                // let count = Number.parseInt(crrEl.innerHTML);
                                // if (!count) {
                                //     count = 0;
                                // }
                                // count++;

                                crrEl.innerHTML = data;
                                crrEl.parentElement.style.display = "block";

                                // remove listener
                                const new_element = el.cloneNode(true);
                                new_element.innerHTML = "Đã thích";
                                new_element.classList.add('vc-comment-liked');
                                el.parentNode.replaceChild(new_element, el);

                                // Save liked log
                                liked.push(commentId);
                                localStorage.setItem(btoa(getCurrentQuery()), JSON.stringify(liked));
                            })
                    })
                }
            })
        });
}

function saveFullName() {
    const fullname = document.querySelector("#vinacent-comment-fullname").value;
    localStorage.setItem("_vinacent.com_comment_box_fullname", fullname);
}

function loadFullName() {
    const fullname = document.querySelector("#vinacent-comment-fullname").value;
    if (!fullname || fullname.length <= 0) {
        document.querySelector("#vinacent-comment-fullname").value = localStorage.getItem("_vinacent.com_comment_box_fullname") ?? "";
    }
}

function initCommentForm() {
    const form = document.querySelector('#VinaCent_Form');
    form.addEventListener("submit", function (e) {
        e.preventDefault();    //stop form from submitting
        fetch(form.action, {
            method: form.method ?? 'POST', body: new FormData(form)
        })
            .then(response => response.text())
            .then(data => {
                document.querySelector('#notify').innerHTML = data;
                saveFullName();
                // reset
                document.querySelector('#vinacent-comment-area').value = '';
                document.querySelector('#vinacent-comment-parent').value = '';
                document.querySelector('#vinacent-comment-image').value = null;

                loadComments();
                setTimeout(() => {
                    document.querySelector("#notify").innerHTML = "";
                }, 3000);
            });
    });
}

function loadCommentBox() {
    fetch(host + 'comments/load-box' + getCurrentQuery())
        .then(response => response.text())
        .then(data => {
            document.querySelector('#VinaCent_CommentBox').innerHTML = data;
            initCommentForm();
            loadFullName();
            loadComments();
        });
}

loadCommentBox();