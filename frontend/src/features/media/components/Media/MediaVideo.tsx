'use client'
import ReactPlayer from 'react-player'

export function MediaVideo ({url}:{url:string}){
  return <ReactPlayer controls={true} playing={false} loop={true} width={'100%'} height={'auto'} url={url}/>
}
