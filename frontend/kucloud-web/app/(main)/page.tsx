'use client';

import { useState } from 'react';

import { Button, Checkbox, Group, ScrollArea, Table, Text, rem } from '@mantine/core';
import { useQuery } from '@tanstack/react-query';
import cx from 'clsx';

import { Api } from '@/lib/api';
import { StorageNodeDto } from '@/lib/models';

import classes from './TableSelection.module.css';

export default function HomePage() {
  const [folderId, setFolderId] = useState<number>(1);
  const [selection, setSelection] = useState<StorageNodeDto[]>([]);
  const { data: folder } = useQuery({
    queryKey: ['folder', folderId],
    queryFn: () => Api.Storage.getFolder({ id: folderId }),
  });

  if (!folder) {
    return <div>Loading...</div>;
  }

  const { children, ancestors } = folder;

  const breadcurmbs = ancestors.map((item) => (
    <>
      <Button
        variant="subtle"
        onClick={() => {
          setFolderId(item.id);
        }}
      >
        {item.name}
      </Button>
      <Text>{'>'}</Text>
    </>
  ));

  const toggleRow = (item: StorageNodeDto) =>
    setSelection((current) =>
      current.includes(item) ? current.filter((item2) => item2 !== item) : [...current, item]
    );
  const toggleAll = () =>
    setSelection((current) =>
      current.length === children.length ? [] : children.map((item) => item)
    );

  const rows = children.map((item) => {
    const selected = selection.includes(item);
    return (
      <Table.Tr key={item.id} className={cx({ [classes.rowSelected]: selected })}>
        <Table.Td>
          <Checkbox checked={selection.includes(item)} onChange={() => toggleRow(item)} />
        </Table.Td>
        <Table.Td
          onClick={() => {
            item.type === 'Folder' && setFolderId(item.id);
          }}
        >
          <Group gap="sm" style={{ cursor: 'pointer' }}>
            <Text size="sm" fw={500}>
              {item.name}
            </Text>
          </Group>
        </Table.Td>
        <Table.Td>{item.size / 1000} KB</Table.Td>
        <Table.Td>
          {(item.auditRecord.modifiedTime ?? item.auditRecord.creationTime).toString()}
        </Table.Td>
      </Table.Tr>
    );
  });

  return (
    <>
      <Group gap="sm">{breadcurmbs}</Group>
      <ScrollArea>
        <Table miw={800} verticalSpacing="sm">
          <Table.Thead>
            <Table.Tr>
              <Table.Th style={{ width: rem(40) }}>
                <Checkbox
                  onChange={toggleAll}
                  checked={selection.length === children.length}
                  indeterminate={selection.length > 0 && selection.length !== children.length}
                />
              </Table.Th>
              <Table.Th>名称</Table.Th>
              <Table.Th>大小</Table.Th>
              <Table.Th>修改时间</Table.Th>
            </Table.Tr>
          </Table.Thead>
          <Table.Tbody>{rows}</Table.Tbody>
        </Table>
      </ScrollArea>
    </>
  );
}
