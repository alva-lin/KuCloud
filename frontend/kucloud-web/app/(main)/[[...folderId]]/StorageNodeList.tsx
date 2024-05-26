import { useEffect, useState } from 'react';

import { Checkbox, Group, ScrollArea, Table, Text, rem } from '@mantine/core';
import cx from 'clsx';

import { StorageNodeDto } from '@/lib/models';
import { formatDate, formatFileSize } from '@/lib/utils';

import classes from './TableSelection.module.css';

export default function StorageNodeList(props: {
  nodes: StorageNodeDto[];
  onClickNextFolder: (id: number) => void;
  onSelectionChange?: (selection: StorageNodeDto[]) => void;
}) {
  const { nodes, onClickNextFolder } = props;

  const [selection, setSelection] = useState<StorageNodeDto[]>([]);
  const toggleRow = (item: StorageNodeDto) =>
    setSelection((current) =>
      current.includes(item) ? current.filter((item2) => item2 !== item) : [...current, item]
    );
  const toggleAll = () => setSelection((current) => (current.length === nodes.length ? [] : nodes));

  useEffect(() => {
    props.onSelectionChange && props.onSelectionChange(selection);
  }, [selection]);

  const rows = nodes.map((item) => {
    const selected = selection.includes(item);
    return (
      <Table.Tr key={item.id} className={cx({ [classes.rowSelected]: selected })}>
        <Table.Td>
          <Checkbox
            checked={selection.length > 0 && selection.includes(item)}
            onChange={() => toggleRow(item)}
          />
        </Table.Td>
        <Table.Td w="40">
          <div className="flex content-center">
            {item.type === 'Folder' ? (
              <span className="i-fluent-folder-24-regular w-6 h-6" role="img" aria-hidden="true" />
            ) : (
              <span
                className="i-fluent-document-24-regular w-6 h-6"
                role="img"
                aria-hidden="true"
              />
            )}
          </div>
        </Table.Td>
        <Table.Td
          onClick={() => toggleRow(item)}
          onDoubleClick={() => {
            item.type === 'Folder' && onClickNextFolder(item.id);
          }}
        >
          <Group gap="sm">
            <Text size="sm" fw={500}>
              {item.name}
            </Text>
          </Group>
        </Table.Td>
        <Table.Td>{formatFileSize(item.size)}</Table.Td>
        <Table.Td>{formatDate(item.auditRecord.lastUpdateTime)}</Table.Td>
      </Table.Tr>
    );
  });

  return (
    <>
      <ScrollArea>
        <Table miw={800} verticalSpacing="sm">
          <Table.Thead>
            <Table.Tr>
              <Table.Th style={{ width: rem(40) }}>
                <Checkbox
                  onChange={toggleAll}
                  checked={selection.length > 0 && selection.length === nodes.length}
                  indeterminate={selection.length > 0 && selection.length !== nodes.length}
                />
              </Table.Th>
              <Table.Th>
                <span
                  className="i-fluent-document-24-regular w-6 h-6"
                  role="img"
                  aria-hidden="true"
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
