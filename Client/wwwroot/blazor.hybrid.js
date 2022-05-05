(function () {
    const logInfo = function () {
        console.info('[BlazorHybrid]:', ...arguments);
    }

    const getElementByTagName = function (name) {
        return document.getElementsByTagName(name)[0];
    }

    const serverApp = getElementByTagName("server-app");
    const wasmApp = getElementByTagName("wasm-app");

    const windowAddEventActual = window.addEventListener;
    const documentAddEventActual = document.addEventListener;

    let wasmNavigateTo = null;
    let captureListeners = false;

    const addServerEvent = function (type, listener, options) {
        serverApp.addEventListener(type, listener, options);
    }

    const wasmListeners = [];
    const addWasmEvent = function (type, listener, options) {
        if (type !== 'click' && type !== 'message' && type !== 'popstate') {
            addServerEvent(type, listener, options);
        }

        if (captureListeners) {
            wasmListeners.push({ type, listener, options });
        }
    }

    const loadScript = function (name, callback) {
        const script = document.createElement('script');
        script.onload = callback;
        if (name === "webassembly") {
            script.setAttribute("autostart", "false");
        }
        script.src = '_framework/blazor.' + name + '.js';
        document.head.appendChild(script);
    }

    const loadWasmFunction = function () {
        if (getElementByTagName('server-app').innerHTML.indexOf('<!--Blazor:{') !== -1) {
            setTimeout(loadWasmFunction, 100);
            return;
        }

        window.addEventListener = addWasmEvent;
        document.addEventListener = addWasmEvent;

        captureListeners = true;

        loadScript('webassembly', function () {
            wasmNavigateTo = window.Blazor._internal.navigationManager.navigateTo;
            window.Blazor._internal.navigationManager.navigateTo = window.BlazorServer._internal.navigationManager.navigateTo;
            window.Blazor.start();
        });
    };

    const init = function () {
        let wasmReadyToSwitch = false;

        window.addEventListener = addServerEvent;
        document.addEventListener = addServerEvent;

        loadScript('server', function () {
            window.BlazorServer = window.Blazor;
        });

        setTimeout(loadWasmFunction, 100);

        window.blazorWasmReady = function () {
            logInfo('WASM ready');
            wasmReadyToSwitch = true;

            if (window.blazorHybridType === 'HybridOnReady') {
                window.blazorSwitchToWasm(window.location.href);
            }
        }

        window.blazorSwitchToWasm = function (location, manual) {
            if (window.blazorHybridType === 'HybridManual' && !manual) {
                return true;
            }

            if (manual) {
                location = window.location.href;
            }

            if (!wasmReadyToSwitch) return false;
            wasmReadyToSwitch = false;

            logInfo('Switching to WASM');

            setTimeout(function () {

                serverApp.parentNode.removeChild(serverApp);

                for (const wasmListener of wasmListeners) {
                    wasmApp.addEventListener(wasmListener.type, wasmListener.listener, wasmListener.options);
                }

                window.wasmListeners = wasmListeners

                window.addEventListener = windowAddEventActual;
                document.addEventListener = documentAddEventActual;

                window.BlazorServer.defaultReconnectionHandler.onConnectionDown = () => { };
                window.BlazorServer._internal.forceCloseConnection();

                window.Blazor._internal.navigationManager.navigateTo = wasmNavigateTo;
                window.addEventListener('popstate', () => {
                    wasmNavigateTo(window.location.href, false, true);
                });

                wasmNavigateTo(location, false, true);

                wasmApp.style.display = "block";
                serverApp.style.display = "none";
            }, 0);

            return true;
        }
    }

    init();

})();
