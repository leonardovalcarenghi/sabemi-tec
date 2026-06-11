export function formatCurrency(value: number, locale = 'pt-BR', currency = 'BRL'): string {
    return new Intl.NumberFormat(locale, { style: 'currency', currency }).format(value)
}

export function formatDate(value: string | Date, locale = 'pt-BR'): string {
    return new Intl.DateTimeFormat(locale, { day: '2-digit', month: '2-digit', year: 'numeric' }).format(
        typeof value === 'string' ? new Date(value) : value,
    )
}

export function toQueryParams(params: any): string {
    const query = new URLSearchParams()
    for (const [key, value] of Object.entries(params)) {
        if (value === null || value === undefined || value === '') continue
        if (Array.isArray(value)) {
            value.forEach((item) => query.append(key, String(item)))
        } else {
            query.set(key, String(value))
        }
    }
    const result = query.toString()
    return result ? `?${result}` : ''
}