import React, {createContext, useContext} from "react";

type MediaGridContextType = {
  selectedMedia : Set<string>
  toggleSelection: (id:string) => void
  clearSelection: () => void
  isSelectionMode: boolean
}

const MediaGridContext = createContext<MediaGridContextType | null>(null)

export function MediaContextProvider({ children }:{children: React.ReactNode}) {
  const [selectedMedia, setSelectedMedia] = React.useState<Set<string>>(new Set())
  
  const toggleSelection = (id:string) => {
    setSelectedMedia( prev => {
      const next = new Set(prev);
      next.has(id) 
        ? next.delete(id)
        : next.add(id)
      return next
    })
  }
  const clearSelection = () => {
    setSelectedMedia(new Set())
  }
  
  return (
    <MediaGridContext.Provider value={{
      selectedMedia: selectedMedia,
      toggleSelection: toggleSelection,
      clearSelection: clearSelection,
      isSelectionMode:selectedMedia.size >= 1 ,
    }}>
      {children}
    </MediaGridContext.Provider>
  )
  
}

export const useMediaGrid = () => {
  const context = useContext(MediaGridContext);
  if(!context) throw new Error('useMediaSelection must be used within MediaContextProvider');
  return context;
}
