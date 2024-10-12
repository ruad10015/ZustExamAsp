﻿var toastId = "myToast";

function createToast(text, color) {
    let toast = `
    <div class="position-fixed bottom-0 end-0 p-3" style="z-index: 11">
      <div id="${toastId}" class="toast" role="alert" aria-live="assertive" aria-atomic="true" style="border: 1px solid ${color}; background-color: white;">
        <div class="toast-header">
          <strong class="me-auto">Zust</strong>
          <small>Now</small>
          <button type="button" class="btn-close" data-bs-dismiss="toast" aria-label="Close"></button>
        </div>
        <div class="toast-body" style="color: ${color};">
          ${text}
        </div>
      </div>
    </div>
  `;
    return toast;
}

// Function to show the toast
function showToast(message, color) {
    // Remove existing toast if it exists
    var existingToast = document.getElementById(toastId);
    if (existingToast) {
        existingToast.remove();
    }

    var toastHTML = createToast(message, color);
    document.body.insertAdjacentHTML("beforeend", toastHTML);
    var toast = document.getElementById(toastId);
    var bsToast = new bootstrap.Toast(toast);
    bsToast.show();
    setTimeout(function () {
        toast.style.display = "none";
    }, 6000);

    // Handle close button click event
    var closeButton = toast.querySelector(".btn-close");
    closeButton.addEventListener("click", function () {
        toast.style.display = "none";
    });
}

function getDateTimeDifference(dateTime) {
    const requestTime = new Date(dateTime);
    const currentTime = new Date();

    // Calculate the time difference in milliseconds
    const difference = Math.abs(currentTime.getTime() - requestTime.getTime());

    // If the time difference is less than 1 second, return "Right now"
    if (difference < 1000) {
        return "Right now";
    }

    // Convert milliseconds to seconds, minutes, hours, days, months, and years
    const seconds = Math.floor(difference / 1000);
    const minutes = Math.floor(seconds / 60);
    const hours = Math.floor(minutes / 60);
    const days = Math.floor(hours / 24);
    const months = Math.floor(days / 30);
    const years = Math.floor(months / 12);

    // Format the result as a string
    if (years > 0) {
        return years + (years === 1 ? " year ago" : " years ago");
    } else if (months > 0) {
        return months + (months === 1 ? " month ago" : " months ago");
    } else if (days > 0) {
        return days + (days === 1 ? " day ago" : " days ago");
    } else if (hours > 0) {
        return hours + (hours === 1 ? " hour ago" : " hours ago");
    } else if (minutes > 0) {
        return minutes + (minutes === 1 ? " minute ago" : " minutes ago");
    } else {
        return seconds + (seconds === 1 ? " second ago" : " seconds ago");
    }
}

function getNoResultHtml(title, message) {
    let content = `
    <div class="empty-icon-container">
      <div class="animation-container">
        <div class="bounce"></div>
        <div class="pebble1"></div>
        <div class="pebble2"></div>
        <div class="pebble3"></div>
      </div>
      <div>
        <h2>${title}</h2>
        <p>${message}</p>
      </div>
    </div>`;
    return content;
}

function getAllFollowings(currentUserId) {
    return new Promise(function (resolve, reject) {
        $.ajax({
            url: `/api/User/GetFollowings?userId=` + currentUserId,
            method: 'GET',
            success: function (data) {
                resolve(data);
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function getAllFollowingsCount(currentUserId) {
    return new Promise(function (resolve, reject) {
        $.ajax({
            url: `/api/User/GetFollowingsCount?userId=` + currentUserId,
            method: 'GET',
            success: function (data) {
                resolve(data);
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function getAllFollowers(currentUserId) {
    return new Promise(function (resolve, reject) {
        $.ajax({
            url: `/api/User/GetFollowers?userId=` + currentUserId,
            method: 'GET',
            success: function (data) {
                resolve(data);
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function getRandomFollowers(currentUserId) {
    return new Promise(function (resolve, reject) {
        $.ajax({
            url: `/api/User/GetRandomFollowers?userId=` + currentUserId,
            method: 'GET',
            success: function (data) {
                resolve(data);
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}


function getAllFollowersCount(currentUserId) {
    return new Promise(function (resolve, reject) {
        $.ajax({
            url: `/api/User/GetFollowersCount?userId=` + currentUserId,
            method: 'GET',
            success: function (data) {
                resolve(data);
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

async function getAllPostLikeCount(currentUserId) {
    return makeAjaxRequest("/api/Post/GetAllPostsLikeCount?userId=" + currentUserId, "GET");
}

async function getSentFriendRequests(id) {
    return new Promise(function (resolve, reject) {
        $.ajax({
            url: `/api/FriendRequest/GetSentFriendRequests?userId=` + id,
            method: 'GET',
            success: function (data) {
                resolve(data);
            },
            error: function (error) {
                alert("Error occurred: " + error.responseText);
                reject(false);
            }
        });
    });
}

function sendFriendRequest(receiverId) {
    return new Promise(function (resolve, reject) {
        $.ajax({
            url: '/api/FriendRequest/AddFriendRequest?receiverId=' + receiverId,
            method: 'POST',
            success: async function (friendRequestNotificiationVm) {
                resolve(friendRequestNotificiationVm);
            },
            error: function (error) {
                alert("Error occurred: " + error.responseText);
                reject(false);
            }
        });
    });
}

function cancelFriendRequest(receiverId) {
    return new Promise(function (resolve, reject) {
        $.ajax({
            url: '/api/FriendRequest/CancelFriendRequest?receiverId=' + receiverId,
            method: 'POST',
            success: function () {
                resolve(true);
            },
            error: function (error) {
                alert("Error occurred: " + error.responseText);
                reject(false);
            }
        });
    });
}

function removeFriend(friendId) {
    return new Promise(function (resolve, reject) {
        $.ajax({
            url: '/api/User/RemoveFriend?friendId=' + friendId,
            method: 'POST',
            success: function () {
                resolve(true);
            },
            error: function (error) {
                alert("Error occurred: " + error.responseText);
                reject(false);
            }
        });
    });
}

function getButtonHtml(user){
    if (user.hasFriendRequestPending) {
        return `<button id='cancel-${user.id}' onclick="callCancelFriendRequest('${user.id}')" class="cancel-btn btn">Cancel Follow Request</button>`;
    } else if (user.isFriend) {
        return `<button id='unfollow-${user.id}' title="Click to stop following" onClick="callRemoveFriend('${user.id}')" class="remove-friend-btn btn">Unfollow</button>`;
    } else {
        return `<button id='follow-${user.id}' onclick="callSendFriendRequest('${user.id}')" type="submit" class="send-request-btn btn">Follow</button>`;
    }
}

    //function getButtonHtml(sentFriendRequests, user) {
    //    let followRequestExists = false;
    //    let acceptedFriendExists = false;
//
//    for (const request of sentFriendRequests) {
//        if (request.receiverId === user.id) {
//            if (request.status === 'Pending') {
//                followRequestExists = true;
//            } else if (request.status === 'Accepted') {
//                acceptedFriendExists = true;
//            }
//        }
//
//        // If both conditions are true, we can break the loop early.
//        if (followRequestExists && acceptedFriendExists) {
//            break;
//        }
//    }
//
//    if (followRequestExists) {
//        return `<button onclick="callCancelFriendRequest('${user.id}')" class="cancel-btn">Cancel Follow Request</button>`;
//    } else if (acceptedFriendExists) {
//        return `<button title="Click to stop following" onClick="callRemoveFriend('${user.id}')" class="remove-friend-btn">Unfollow</button>`;
//    } else {
//        return `<button onclick="callSendFriendRequest('${user.id}')" type="submit" class="send-request-btn">Follow</button>`;
//    }
//}


function getButtonText(sentFriendRequests, user) {
    let followRequestExists = false;
    let acceptedFriendExists = false;

    for (const request of sentFriendRequests) {
        if (request.receiverId === user.id) {
            if (request.status === 'Pending') {
                followRequestExists = true;
            } else if (request.status === 'Accepted') {
                acceptedFriendExists = true;
            }
        }

        // If both conditions are true, we can break the loop early.
        if (followRequestExists && acceptedFriendExists) {
            break;
        }
    }

    if (followRequestExists) {
        return 'Cancel Follow Request';
    } else if (acceptedFriendExists) {
        return 'Unfollow';
    } else {
        return 'Follow';
    }
}


function getIconClass(user) {
    if (user.hasFriendRequestPending) {
        return 'yellow-icon';
    } else if (user.isFriend) {
        return 'red-icon';
    } else {
        return 'main-icon';
    }
}

//function getIconClass(sentFriendRequests, user) {
//    if (sentFriendRequests.some(i => i.receiverId === user.id && i.status === 'Pending')) {
//        return 'yellow-icon';
//    } else if (sentFriendRequests.some(i => i.receiverId === user.id && i.status === 'Accepted')) { 
//        return 'red-icon';
//    } else {
//        return 'main-icon';
//    }
//}


 async function setSocialCounts(followerElementId, followingElementId, postLikeElementId, currentUserId) {
    var followerElement = document.getElementById(followerElementId);
    var followingElement = document.getElementById(followingElementId);
    var postLikeElement = document.getElementById(postLikeElementId);

    var spinnerHtml = `
        <div class="spinner-border text-primary" role="status" style="color:var(--main-color) !important; width:1.1rem; height:1.1rem; ">
            <span class="sr-only"></span>
        </div>
    `;

    followerElement.innerHTML=  spinnerHtml;
    followingElement.innerHTML = spinnerHtml;
    postLikeElement.innerHTML = spinnerHtml;


    const followerCount = await getAllFollowersCount(currentUserId);
    followerElement.innerHTML = followerCount;
    
    const followingCount = await getAllFollowingsCount(currentUserId);
    followingElement.innerHTML = followingCount;
    
    const postLikeCount = await getAllPostLikeCount(currentUserId);
    postLikeElement.innerHTML = postLikeCount;
    //getAllFollowersCount(currentUserId)
    //.then(data => {
    //    alert(data);
    //    followerElement.innerHTML = data;
    //});
    //
    //    getAllFollowingsCount(currentUserId)
    //.then(data => {
    //    alert(data);
    //
    //    followingElement.innerHTML = data;
    //});
    //
    //    getAllPostLikeCount(currentUserId)
    //.then(data => {
    //    alert(data);
    //
    //    postLikeElement.innerHTML = data;
    //});
}

function makeAjaxRequest(url, type) {
    return new Promise(function (resolve, reject) {
        $.ajax({
            url: url,
            type: type,
            success: function (response) {
                resolve(response);
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}