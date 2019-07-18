// Lave plus blanc que blanc

let headers = new Headers();
headers.append("Accept-Language", document.querySelector('html')!.lang);
headers.append("Content-Type", "application/json");

export const AjaxService = {
    post<Tin, Tout>(route: RequestInfo, data: Tin): Promise<Tout | number>
    {
        const promise = new Promise<Tout | number>((resolve, reject) =>
        {
            fetch(route,
                {
                    headers: headers,
                    method: "post",
                    body: JSON.stringify(data)
                })
                .then((response) =>
                {
                    if (response.ok)
                    {
                        resolve(response.json());
                    }                    
                    else
                    {
                        reject(response.status);
                    }
                });
        });

        return promise;
    },
    put<Tin>(route: RequestInfo, data: Tin): Promise<boolean>
    {
        const promise = new Promise<boolean>((resolve, reject) =>
        {
            fetch(route,
                {
                    headers: headers,
                    method: "post",
                    body: JSON.stringify(data)
                })
                .then((response) =>
                {
                    if (response.ok)
                    {
                        resolve(true);
                    }
                    else
                    {
                        reject(false);
                    }
                });
        });

        return promise;
    },
    get<Tout>(route: RequestInfo): Promise<Tout>
    {
        const promise = new Promise<Tout>((resolve, reject) =>
        {
            return fetch(route,
                {
                    headers: headers,
                    method: "get"
                })
                .then((response) =>
                {
                    if (response.ok)
                    {
                        resolve(response.json());
                    }
                    else
                    {
                        reject(response.status);
                    }
                });
        });

        return promise;
    }
}