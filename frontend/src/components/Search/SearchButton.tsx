'use client'
import {Box, Center, Pill, UnstyledButton} from "@mantine/core";
import {spotlight} from "@mantine/spotlight";
import {IconSearch} from "@tabler/icons-react";
import styles from "./SearchButton.module.css"
import {SearchSpotlight} from "@/src/components/Search/SearchSpotlight";
import React from "react";

export default function SearchButton (){
  return (
    <>
      <SearchSpotlight/>
      <UnstyledButton
        className={styles.button}
        onClick={spotlight.open}>
        <Center className={styles.spaceBetween}>
          <IconSearch  className={styles.icon} />
          <Box
            visibleFrom={'sm'}
            flex={1}
          >
            Search</Box>
          <Pill
            visibleFrom={'sm'}
            radius={"sm"}  className={styles.shortcut}>Ctrl + K</Pill>
        </Center>

      </UnstyledButton>
    </>
  )
}
