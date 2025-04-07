export const getBaseUrl = (): string => {
  return import.meta.env.MODE === 'production'
    ? import.meta.env.VITE_PROD_API || `https://api-${window.location.hostname}/`
    : import.meta.env.VITE_DEV_API || 'https://localhost:7059/'
}
