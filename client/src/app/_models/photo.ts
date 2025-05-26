export interface Photo {
    id: number
    url: string
    isMain: boolean
    isApproved: boolean
    username?: string
    tags:Tag[];
}

export interface Tag
    {
        id:number;
        name:string;
    }