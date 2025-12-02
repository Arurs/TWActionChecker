// ==UserScript==
// @name         WwwwQListCqrs
// @namespace    http://tampermonkey.net/
// @version      0.4
// @description  Zbiera wszystkie ataki i wysyła listę do API z retry 3x (GM_xmlhttpRequest - omija CORS)
// @author       You
// @include      https://pl*.plemiona.pl/game.php?*screen=info_village*
// @include      https://pl*.plemiona.pl/game.php?*screen=overview*
// @icon         https://www.google.com/s2/favicons?domain=plemiona.pl
// @grant        GM_xmlhttpRequest
// @connect      localhost
// @connect      127.0.0.1
// ==/UserScript==

class Attack {
    constructor(destination, player, time, type, source = "nie istotne", comebackTime = "nie istotne") {
        this.destination = destination;
        this.player = player;
        this.time = time;
        this.type = type;
        this.source = source;
        this.comebackTime = comebackTime;
    }
}

(async function () {
    'use strict';

    const apiUrl = 'https://localhost:7154/api/AllyAttack/AddList';

    const sleep = (ms) => new Promise(res => setTimeout(res, ms));

    const getAttackType = (details) => {
        if (!details) return "unknown";
        const html = details.innerHTML || details;
        if (html.includes('attack_small.png')) return "fake";
        if (html.includes('attack_large.png')) return "large";
        if (html.includes('attack_medium.png')) return "medium";
        return "unknown";
    };

    const parseCommandRow = (row) => {
        const firstColumn = row.querySelector("td:nth-child(1)");
        const secondColumn = row.querySelector("td:nth-child(2)");
        const details = firstColumn ? firstColumn.querySelector(".command_hover_details") : null;

        const type = details ? getAttackType(details) : "unknown";

        const villageNameElement = firstColumn ? firstColumn.querySelector(".quickedit-label") : null;
        const playerName = villageNameElement ? villageNameElement.textContent.trim().split(':')[0] : "Unknown Player";

        const timeText = secondColumn ? secondColumn.textContent.trim() : "Unknown Time";

        const destination =
            document.querySelector('#content_value > table > tbody > tr > td:nth-child(1) > table:nth-child(1) > tbody > tr:nth-child(4) > td:nth-child(2)')?.innerText ||
            document.querySelector('#content_value > table > tbody > tr > td:nth-child(1) > table:nth-child(1) > tbody > tr:nth-child(3) > td:nth-child(2)')?.innerText ||
            "Unknown Destination";

        return { attack: new Attack(destination, playerName, timeText, type) };
    };

    //
    // ---  GM_xmlhttpRequest z retry 3 razy ---
    //
    const sendAttackListGM = (attackList) => {
        return new Promise((resolve, reject) => {
            GM_xmlhttpRequest({
                method: "POST",
                url: apiUrl,
                headers: {
                    "Content-Type": "application/json",
                    "Accept": "application/json"
                },
                data: JSON.stringify(attackList),
                onload: function (res) {
                    if (res.status >= 200 && res.status < 300) {
                        resolve(res.responseText);
                    } else {
                        reject(new Error(`GM Status ${res.status}`));
                    }
                },
                onerror: err => reject(err),
                ontimeout: () => reject(new Error("GM timeout")),
                timeout: 30000
            });
        });
    };

    const sendAttackListGMWithRetry = async (attackList, maxRetries = 3) => {
        for (let attempt = 1; attempt <= maxRetries; attempt++) {
            try {
                console.log(`GM Request attempt ${attempt}/${maxRetries}...`);
                const result = await sendAttackListGM(attackList);
                console.log("GM Request success!");
                return result;
            } catch (e) {
                console.error(`GM Request failed (attempt ${attempt}):`, e);
                if (attempt === maxRetries) throw e;
                await sleep(1000); // opóźnienie między próbami
            }
        }
    };

    //
    // ---  Fetch fallback z retry 3 razy ---
    //
    const sendAttackListFallback = async (attackList) => {
        const resp = await fetch(apiUrl, {
            method: "POST",
            headers: { "Content-Type": "application/json", "Accept": "application/json" },
            body: JSON.stringify(attackList)
        });

        if (!resp.ok) throw new Error(`Fetch Status ${resp.status}`);
        return resp.json();
    };

    const sendAttackListFallbackWithRetry = async (attackList, maxRetries = 3) => {
        for (let attempt = 1; attempt <= maxRetries; attempt++) {
            try {
                console.log(`Fetch Request attempt ${attempt}/${maxRetries}...`);
                const result = await sendAttackListFallback(attackList);
                console.log("Fetch Request success!");
                return result;
            } catch (e) {
                console.error(`Fetch Request failed (attempt ${attempt}):`, e);
                if (attempt === maxRetries) throw e;
                await sleep(1000);
            }
        }
    };

    //
    // --- Główna logika ---
    //
    const rows = document.querySelectorAll("#commands_outgoings .command-row");
    const attackList = [];

    for (const row of rows) {
        const { attack } = parseCommandRow(row);
        attackList.push(attack);
    }

    console.log("Wysyłanie listy ataków...");

    try {
        if (typeof GM_xmlhttpRequest === "function") {
            await sendAttackListGMWithRetry(attackList);
        } else {
            await sendAttackListFallbackWithRetry(attackList);
        }
        console.log("Wysyłanie zakończone sukcesem!");
    } catch (err) {
        console.error("❌ Błąd po 3 próbach:", err);
    }

    await sleep(1000);
    close();
})();
