//function someMethod()
//{
//    return "From JS";
//}
//window.someMethod = function ()
//{
//    return "From JS";
//}
//window.someMethod = () =>
//{
//    return "From JS";
//}



/*修改textarea回车行为*/
registerKeyPressHandler = (Input) => {
    const textarea = Input instanceof Element ? Input : document.querySelector(Input);

    textarea.addEventListener("keydown", function (event) {
        if (event.key === "Enter") {

            event.preventDefault(); // 阻止默认的Enter键行为（换行）

            if (event.shiftKey) {
                // 插入换行符
                let text = textarea.value;
                let start = textarea.selectionStart;
                let end = textarea.selectionEnd;
                textarea.value = text.substring(0, start) + "\n" + text.substring(end);
                textarea.selectionStart = textarea.selectionEnd = start + 1;
            }
        }
    });
    textarea.addEventListener("keyup", function (event) {
        if (event.key === "Enter") {
            //高度调一下
            textarea.style.height = 'auto';
            textarea.style.height = `${textarea.scrollHeight}px`;
        }
    });

    //滚动条穿模无解，只好给textarea安排个外容器边框改色
    var container = document.querySelector('.textbox-container');
    textarea.addEventListener('focus', function () {
        container.style.borderColor = '#9D2589';
        container.style.boxShadow = '0 0 2px #A5A5A5';
    });
    textarea.addEventListener('blur', function () {
        container.style.borderColor = '#ccc';
        container.style.boxShadow = 'none';
    });
};

/*主动重置焦点*/
resetFocusToTextarea = (Input) => {
    const textarea = Input instanceof Element ? Input : document.querySelector(Input);

    textarea.blur();
    textarea.focus();
};

/*适应高度*/
resizeTextarea = (Input) => {
    const textarea = Input instanceof Element ? Input : document.querySelector(Input);

    textarea.style.height = 'auto';
    textarea.style.height = `${textarea.scrollHeight}px`;
};

// 页面滚动到最顶部
scrollToTop = async () => {
    window.scrollTo(0, 0)
    return await waitForScrollComplete();
}

// 页面滚动到最底部
scrollToBottom = async () => {
    window.scrollTo(0, document.body.scrollHeight)
    return await waitForScrollComplete();
}
//等待滚动完成
let scrollResolveTimeout;
waitForScrollComplete = () => {
    return new Promise((resolve) => {
        const scrollTimeout = setTimeout(() => {
            resolve(true);
        }, 100);
        const scrollTimeoutEvent = addEventListener('scroll', function fn(e) {
            clearTimeout(scrollTimeout);
            clearTimeout(scrollResolveTimeout);
            scrollResolveTimeout = setTimeout(function () {
                //console.log('スクロールが完了しました！');
                removeEventListener('scroll', fn);
                resolve(true);
            }, 100);
        });
    });
}


//获取InnerText
getInnerText = (element) => {
    //return element.innerText;
    return new Promise((resolve, reject) => {
        try {
            const innerText = element.innerText;
            resolve(innerText);
        } catch (error) {
            //reject("getInnerText error");
        }
    });
};
//设置InnerText
setInnerText = (element, text) => {
    //element.innerText = text;
    return new Promise((resolve, reject) => {
        try {
            element.innerText = text;
            //const wrappedText = `<span>${text}</span>`;
            //element.innerHTML = wrappedText;
            resolve();
        } catch (error) {
            //reject("setInnerText error");
        }
    });
};
//设置焦点
setFocus = (element) => { 
    element.focus();
};
//取消焦点
setBlur = (element) => {
    element.blur();
};
//设置不透明度
setOpacity = (element, opacity) => {
    //element.style.opacity = opacity;
    return new Promise((resolve, reject) => {
        try {
            element.style.opacity = opacity;
            resolve();
        } catch (error) {
            //reject("setOpacity error");
        }
    });
};
//获取光标位置
getCursorPosition = (element) => {
    const selection = window.getSelection();
    const range = selection.getRangeAt(0);
    const preSelectionRange = range.cloneRange();
    preSelectionRange.selectNodeContents(element);
    preSelectionRange.setEnd(range.startContainer, range.startOffset);
    const cursorOffset = preSelectionRange.toString().length;
    return cursorOffset;
};
//设置光标位置
setCursorPosition = (element, position) => {
    const textNode = element.firstChild;
    try {
        if (position < 0) {
            position = 0;
        } else if (position > textNode.length) {
            position = textNode.length;
        }

        const range = document.createRange();
        const selection = window.getSelection();
        range.setStart(textNode, position);
        range.collapse(true);
        selection.removeAllRanges();
        selection.addRange(range);
        element.focus();
    } catch (error) {
        //console.error("Error while setting cursor position:", error);
    }
};

//获取光标位置ex
getCursorPositionEx = (element) => {
    const selection = window.getSelection();
    const range = selection.getRangeAt(0);
    const preSelectionRange = range.cloneRange();
    preSelectionRange.selectNodeContents(element);
    preSelectionRange.setEnd(range.startContainer, range.startOffset);

    const lines = preSelectionRange.toString().split('\n');
    const lineIndex = lines.length - 1;
    const columnIndex = lines[lineIndex].length;

    let emptyLineCount = 0;//计算光标之前有多少空行
    for (const line of lines) {
        if (line.trim() === '') {
            emptyLineCount++;
        }
    }

    return { lineIndex: lineIndex, columnIndex: columnIndex, emptylinecount: emptyLineCount };
};
//设置光标位置ex
setCursorPositionEx = (element, lineIndex, targetPosition) => {
    var textNodes = [];

    // Collect all the text nodes within the element
    function getTextNodes(node) {
        if (node.nodeType === Node.TEXT_NODE) {
            textNodes.push(node);
        } else {
            for (var i = 0; i < node.childNodes.length; i++) {
                getTextNodes(node.childNodes[i]);
            }
        }
    }
    getTextNodes(element);

    if (lineIndex >= 0 && lineIndex < textNodes.length) {
        var targetNode = textNodes[lineIndex];

        if (targetPosition >= 0) {
            var maxPosition = targetNode.nodeValue.length;
            if (targetPosition > maxPosition) {
                targetPosition = maxPosition; // Adjust to not go beyond the text length
            }

            var range = document.createRange();
            var sel = window.getSelection();
            range.setStart(targetNode, targetPosition);
            range.collapse(true);
            sel.removeAllRanges();
            sel.addRange(range);
        }
    }
};

//计算文本节点数量（空行不算...）
getTextNodesLength = (element) => {
    var textNodes = [];
    // Collect all the text nodes within the element
    function getTextNodes(node) {
        if (node.nodeType === Node.TEXT_NODE) {
            textNodes.push(node);
        } else {
            for (var i = 0; i < node.childNodes.length; i++) {
                getTextNodes(node.childNodes[i]);
            }
        }
    }
    getTextNodes(element);

    return textNodes.length;
}



//html2canvas截图用（弃用，改为使用Puppeteer来截图）
takeScreenshot = async (id) => {
    //
    var element = document.querySelector("#" + id);

    // 第一次截图，仅获取截图的宽高信息
    var tempCanvas = await html2canvas(element, {
        backgroundColor: null
    });
    // 获取第一次截图的宽高
    var width = tempCanvas.width;
    var height = tempCanvas.height;

    // 创建一个临时的 Canvas 元素
    var canvas = document.createElement("canvas");
    var ctx = canvas.getContext("2d");

    // 设置 Canvas 大小与元素相同
    canvas.width = width;
    canvas.height = height;

    // 创建渐变对象并设置渐变样式
    var gradient = ctx.createLinearGradient(0, 0, canvas.width, 0); // 将第二个参数设为 Canvas 的宽度
    gradient.addColorStop(0, "#eeedf3");
    gradient.addColorStop(1, "#dddffc");

    // 使用渐变样式填充 Canvas
    ctx.fillStyle = gradient;
    ctx.fillRect(0, 0, canvas.width, canvas.height);

    // 使用 html2canvas 进行截图
    var img = "";
    await html2canvas(element, {
        backgroundColor: null,
        canvas: canvas // 将之前创建的 Canvas 对象传递给 html2canvas
    }).then(c => img = c.toDataURL("image/png"));

    var d = document.createElement("a");
    d.href = img;
    d.download = "image.png";
    d.click();

    return img;
}


//base64转blob
base64ToBlob = (code) => {
    let parts = code.split(';base64,');
    let contentType = parts[0].split(':')[1];
    let raw = window.atob(parts[1]);
    let rawLength = raw.length;

    let uInt8Array = new Uint8Array(rawLength);

    for (let i = 0; i < rawLength; ++i) {
        uInt8Array[i] = raw.charCodeAt(i);
    }
    return new Blob([uInt8Array], { type: contentType });
}
//下载base64截图
downloadbase64img = async (fileName, content) => {
    let aLink = document.createElement('a');
    aLink.download = fileName;
    aLink.href = URL.createObjectURL(this.base64ToBlob(content));
    aLink.click()
}


//其他
getClientWidth = () => {
    return document.body.clientWidth;
}
getScrollHeight = () => {
    return document.body.scrollHeight;
}
getSystemScaleFactor = () => {
    return window.devicePixelRatio;
}









