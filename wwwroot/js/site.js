window.addEventListener('load', eventWindowLoaded, false);

function eventWindowLoaded() {
	canvasApp();
}
function canvasSupport(e) {

	return !!e.getContext;

}
function canvasApp() {
	var canvas = document.getElementById('backgroundCanvas');

	if (!canvasSupport(canvas)) {
		return;
	}

	var ctx = canvas.getContext('2d');
	var w = canvas.width = window.innerWidth;
	var h = canvas.height = window.innerHeight;
	var yPositions = Array(500).join(0).split('');

	function runMatrix() {
		if (typeof Game_Interval != 'undefined') clearInterval(Game_interval);
		Game_Interval = setInterval(drawScreen, 33);
	}

	function drawScreen() {
		ctx.fillStyle = 'rgba(0,0,0,.05)';
		ctx.fillRect(0, 0, w, h);
		ctx.fillStyle = '#0f0';
		ctx.font = '8px Georgia';
		yPositions.map(function (y, index) {
			text = String.fromCharCode(12500 + Math.random() * 100);
			x = (index * 10) + 10;
			ctx.fillText(text, x, y);
			if (y > 500 + Math.random() * 1e4) {
				yPositions[index] = 0;
			} else {
				yPositions[index] = y + 10;
			}
		})
	}

	runMatrix();

}

$(document).ready(function () {
	var name = $("#name");
	var email = $("#email");
	var message = $("#message");
	var notificationWell = $(".notification-well");
	var notificationTemplate = $(".notification-template");
	var opened = new Date();
	$('#send-email').click(function (e) {
		e.preventDefault();
		var errors = validate();
		if ((new Date() - opened) / 1000 < 10) {
			return;
        }
		if (errors.length > 0) {
			showMessages(errors, true);
			return;
        }

		$.ajax({
			url: "/api/Email",
			method: "POST",
			contentType: "application/json",
			data: JSON.stringify({ Name: name.val(), Email: email.val(), Message: message.val() }),
			success: function (isSuccess) {
				if (isSuccess) {
					showMessages(["Contact message sent successfully"], false);
				} else {
					showMessages(["An unexpected error occured, please try again later"], true);
				}
				name.val('');
				email.val('');
				message.val('');
				opened = new Date()
			},
			error: function () {
				showMessages(["An unexpected error occured, please try again later"], true);
            }
		});
	})
	function validate() {
		var errors = [];
		if (name.val() == '') {
			errors.push("Contact name is required")
		}
		if (email.val() == '') {
			errors.push("Contact email is required")
		}
		else if (!(/^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$/i.test(email.val()))) {
			errors.push("Please enter a valid email address")
        }
		if (message.val() == '') {
			errors.push("Message is required")
		}

		return errors;
	}
	function showMessages(messages, isError) {
		$.each(messages, function (index, message) {
			var notification = notificationTemplate.html();
			var key = makeId(15);
			notification = notification.replace('%key%', key)
			notification = notification.replace('%type%', isError ? 'error' : 'success')
			notification = notification.replace('%message%', message)
			notificationWell.append(notification);
			setTimeout(function () {
				notificationWell.find('.notification[data-key=' + key + ']').remove();
            }, 10*1000) //10s
        })
	}
	function makeId(length) {
		var result = '';
		var characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
		var charactersLength = characters.length;
		for (var i = 0; i < length; i++) {
			result += characters.charAt(Math.floor(Math.random() * charactersLength));
		}
		return result;
	}
});