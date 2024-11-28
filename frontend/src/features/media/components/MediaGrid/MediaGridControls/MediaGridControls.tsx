'use client'
import {Center, FloatingIndicator, Tooltip, UnstyledButton} from "@mantine/core";
import React from "react";
import styles from "./MediaGridControls.module.css"
import {GRID_TYPES} from "@/src/features/media/components/MediaGrid/MediaGrid";
import {useRouter, useSearchParams} from "next/navigation";
import { IconGridDots, IconLayout2Filled} from "@tabler/icons-react";

export function MediaGridControls(){
  const router = useRouter()
  const searchParams = useSearchParams()
  
  const [rootRef, setRootRef] = React.useState<HTMLDivElement | null>(null)
  const [controlsRefs, setControlsRefs] = React.useState<Record<string, HTMLButtonElement | null>>({})
  
  const activeIndex = React.useMemo(() => {
    const flowParam = Number(searchParams.get('flow'))
    return flowParam ? flowParam : 1;
  },[searchParams])
  const handleClick = (idx: number) => {
    router.replace(`/?flow=${idx}` )
  }
  
  const setControlRef = (idx: number) => (node: HTMLButtonElement) => {
    controlsRefs[idx] = node;
    setControlsRefs(controlsRefs);
  }

  
  const controls = Object.entries( GRID_TYPES).map(([k, item]) => {
    const idx = Number(k)
    const icon = item === 'Even'
        ? <IconGridDots className={styles.controlLabel}/>
        : item === 'Masonry'
        ? <IconLayout2Filled className={styles.controlLabel} />
        : null
    if (!icon)
      throw new Error("Unhandled gird type in MediaGridControls")
      
    
    return(
      <UnstyledButton
      key={item}
      className={styles.control}
      ref={setControlRef(idx)}
      onClick={() => handleClick(idx)}
      mod={{active: activeIndex === idx}}
      >
        <Tooltip label={item}>
          <Center>

            { icon }
          </Center>
        </Tooltip>
      </UnstyledButton>
    )
  });
  
  return (
    <div className={styles.root} ref={setRootRef}>
      {controls}
      <FloatingIndicator
        target={controlsRefs[activeIndex]}
        parent={rootRef}
        className={styles.indicator}
      >
      </FloatingIndicator>
    </div>
  )
}
